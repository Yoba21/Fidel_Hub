using School.Shared.Tenancy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register scoped multi-tenant context
builder.Services.AddScoped<ICurrentTenantContext, CurrentTenantContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Multi-tenant resolution middleware
app.Use(async (context, next) =>
{
    var tenantContext = context.RequestServices.GetRequiredService<ICurrentTenantContext>();
    
    // Resolve tenant from header or default development tenant
    if (context.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader) &&
        Guid.TryParse(tenantHeader, out var tenantId))
    {
        tenantContext.SetTenant(tenantId);
    }
    else
    {
        // Default seed demo tenant (St. Joseph Academy)
        tenantContext.SetTenant(Guid.Parse("11111111-1111-1111-1111-111111111111"), "stjoseph");
    }

    await next();
});

app.MapGet("/api/health", (ICurrentTenantContext tenant) => Results.Ok(new
{
    status = "Healthy",
    platform = "Fidel Hub School Management SaaS",
    activeTenant = tenant.TenantId,
    timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithOpenApi();

app.Run();
