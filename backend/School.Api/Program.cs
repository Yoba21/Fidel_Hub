using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using School.Shared.Data;
using School.Shared.Data.Interceptors;
using School.Shared.Tenancy;

var builder = WebApplication.CreateBuilder(args);

// 1. Core Services & Tenancy
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ICurrentTenantContext, CurrentTenantContext>();
builder.Services.AddScoped<TenantConnectionInterceptor>();

// 2. Database Context with Multi-Tenant Interceptor
builder.Services.AddDbContext<FidelHubDbContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("Postgres")
        ?? "Host=localhost;Port=5432;Database=fidelhub;Username=fidel_app_user;Password=fidel_secure_pass";
    
    var interceptor = sp.GetRequiredService<TenantConnectionInterceptor>();
    options.UseNpgsql(connectionString)
           .AddInterceptors(interceptor);
});

// 3. JWT Bearer Authentication (Better Auth Integration)
var jwtIssuer = builder.Configuration["BetterAuth:Issuer"] ?? "https://auth.fidelhub.com";
var jwtAudience = builder.Configuration["BetterAuth:Audience"] ?? "fidel-hub-api";
var jwtSigningKey = builder.Configuration["BetterAuth:SecretKey"] ?? "fidel_hub_temporary_development_secret_key_minimum_32_characters";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtIssuer;
        options.Audience = jwtAudience;
        options.RequireHttpsMetadata = false; // Set to true in strict production
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false, // Allow multi-client tokens
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)),
            ClockSkew = TimeSpan.FromSeconds(5)
        };
    });

builder.Services.AddAuthorization();

// 4. Swagger with Bearer Authentication Configuration
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

// 5. Tenant & Branch Resolution Middleware
app.Use(async (context, next) =>
{
    var tenantContext = context.RequestServices.GetRequiredService<ICurrentTenantContext>();

    // A. Check Authenticated JWT Claims first
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var tenantClaim = context.User.FindFirst("tenant_id")?.Value;
        var branchClaim = context.User.FindFirst("branch_id")?.Value;
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? context.User.FindFirst("sub")?.Value;

        if (Guid.TryParse(tenantClaim, out var tenantId))
        {
            Guid? branchId = Guid.TryParse(branchClaim, out var bId) ? bId : null;
            Guid? userId = Guid.TryParse(userIdClaim, out var uId) ? uId : null;

            var roles = context.User.FindAll(ClaimTypes.Role).Concat(context.User.FindAll("roles")).Select(c => c.Value);
            var permissions = context.User.FindAll("permissions").Select(c => c.Value);

            tenantContext.SetContext(tenantId, branchId, userId, roles, permissions);
        }
    }
    // B. Development / Test Header Fallback
    else if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader) &&
             Guid.TryParse(tenantHeader, out var devTenantId))
    {
        Guid? devBranchId = context.Request.Headers.TryGetValue("X-Branch-ID", out var branchHeader) &&
                            Guid.TryParse(branchHeader, out var bId) ? bId : null;
        
        tenantContext.SetContext(devTenantId, devBranchId, null, null, null, "fidelhub");
    }
    // C. Default Seed Tenant Fallback (Fidel Hub Academy)
    else
    {
        tenantContext.SetContext(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            null,
            new[] { "PLATFORM_SUPERADMIN" },
            null,
            "fidelhub"
        );
    }

    await next();
});

app.UseAuthorization();

// 6. Base API Endpoints
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
.WithOpenApi();

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

    return Results.Ok(tenant);
})
.WithName("GetCurrentTenant")
.WithOpenApi();

app.Run();
