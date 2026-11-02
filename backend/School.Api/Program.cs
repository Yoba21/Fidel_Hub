using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using School.Api.Contracts;
using School.Shared.Data;
using School.Shared.Data.Interceptors;
using School.Shared.Tenancy;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Services & Tenancy
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ICurrentTenantContext, CurrentTenantContext>();
builder.Services.AddScoped<TenantConnectionInterceptor>();

// 2. CORS Policy for Frontend Client
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000", "https://app.fidelhub.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// 3. Database Context with Multi-Tenant Interceptor
builder.Services.AddDbContext<FidelHubDbContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'Postgres' is not configured.");
    
    var interceptor = sp.GetRequiredService<TenantConnectionInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});

// 4. JWT Bearer Authentication (Better Auth Integration)
var jwtIssuer = builder.Configuration["BetterAuth:Issuer"] ?? "http://localhost:4000";
var jwtAudience = builder.Configuration["BetterAuth:Audience"] ?? "fidel-hub-api";
var metadataAddress = builder.Configuration["BetterAuth:MetadataAddress"] ?? "http://localhost:4000/.well-known/openid-configuration";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtIssuer;
        options.Audience = jwtAudience;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.MetadataAddress = metadataAddress;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

builder.Services.AddAuthorization();

// 5. Swagger with Bearer Authentication Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Fidel Hub Core API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// 6. Tenant & Branch Resolution Middleware
app.Use(async (context, next) =>
{
    var tenantContext = context.RequestServices.GetRequiredService<ICurrentTenantContext>();

    // A. Authenticated JWT Claims (Strict primary source)
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var tenantClaim = context.User.FindFirst("tenant_id")?.Value;
        if (string.IsNullOrEmpty(tenantClaim) || !Guid.TryParse(tenantClaim, out var tenantId))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "Unauthorized: tenant_id claim is missing or invalid in authenticated token." });
            return;
        }

        var branchClaim = context.User.FindFirst("branch_id")?.Value;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("sub")?.Value;

        Guid? branchId = Guid.TryParse(branchClaim, out var bId) ? bId : null;
        Guid? userId = Guid.TryParse(userIdClaim, out var uId) ? uId : null;

        var roles = context.User.FindAll(ClaimTypes.Role)
            .Concat(context.User.FindAll("roles"))
            .Select(c => c.Value);
        var permissions = context.User.FindAll("permissions")
            .Select(c => c.Value);

        tenantContext.SetContext(tenantId, branchId, userId, roles, permissions);
    }
    // B. Development / Test Header Scoping (Only for non-privileged tenant resolution)
    else if (app.Environment.IsDevelopment() &&
             context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader) &&
             Guid.TryParse(tenantHeader, out var devTenantId))
    {
        app.Logger.LogWarning("Development override: Unauthenticated X-Tenant-ID used for tenant {TenantId}. Assigning unprivileged context.", devTenantId);
        Guid? devBranchId = context.Request.Headers.TryGetValue("X-Branch-ID", out var branchHeader) &&
                            Guid.TryParse(branchHeader, out var bId) ? bId : null;
        
        tenantContext.SetContext(devTenantId, devBranchId, null, Array.Empty<string>(), Array.Empty<string>(), "dev");
    }

    await next();
});

app.UseAuthorization();

// 7. Base Health & Observability Endpoints (Kubernetes & Docker Probes)
app.MapGet("/health/live", () => Results.Ok(new { status = "Live", timestamp = DateTime.UtcNow }))
    .WithName("LivenessProbe")
    .AllowAnonymous();

app.MapGet("/health/ready", async (FidelHubDbContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        return canConnect
            ? Results.Ok(new { status = "Ready", database = "Connected", timestamp = DateTime.UtcNow })
            : Results.StatusCode(503);
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "Unhealthy", error = ex.Message }, statusCode: 503);
    }
})
.WithName("ReadinessProbe")
.AllowAnonymous();

app.MapGet("/api/health", (ICurrentTenantContext tenant) => Results.Ok(new
{
    status = "Healthy",
    platform = "Fidel Hub School Management SaaS",
    activeTenant = tenant.TenantId,
    activeBranch = tenant.BranchId,
    isResolved = tenant.IsResolved,
    timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck")
.AllowAnonymous();

// 8. Protected API Endpoints (Requires Valid Tenant JWT)
app.MapGet("/api/v1/auth/me", (ICurrentTenantContext tenant, ClaimsPrincipal user) =>
{
    return Results.Ok(new
    {
        isAuthenticated = user.Identity?.IsAuthenticated ?? false,
        tenantId = tenant.TenantId,
        tenantCode = tenant.TenantCode,
        branchId = tenant.BranchId,
        userId = tenant.UserId,
        roles = tenant.Roles,
        permissions = tenant.Permissions
    });
})
.WithName("GetCurrentUserContext")
.RequireAuthorization()
.WithOpenApi();

app.MapGet("/api/v1/tenants/current", async (FidelHubDbContext db, ICurrentTenantContext tenantContext) =>
{
    if (!tenantContext.TenantId.HasValue)
        return Results.NotFound(new { error = "Tenant context not resolved." });

    var tenant = await db.Tenants
        .AsNoTracking()
        .FirstOrDefaultAsync(t => t.Id == tenantContext.TenantId.Value);

    if (tenant == null)
        return Results.NotFound(new { error = "Tenant not found." });

    return Results.Ok(new TenantResponse(
        tenant.Id,
        tenant.Name,
        tenant.Code,
        tenant.LegalName,
        tenant.Email,
        tenant.Phone,
        tenant.Status,
        tenant.DefaultCurrency,
        tenant.CreatedAt,
        tenant.UpdatedAt));
})
.WithName("GetCurrentTenant")
.RequireAuthorization()
.WithOpenApi();

app.Run();
