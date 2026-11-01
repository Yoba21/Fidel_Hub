using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using School.Shared.Entities;
using School.Shared.Outbox;
using School.Shared.Primitives;
using School.Shared.Tenancy;

namespace School.Shared.Data;

public class FidelHubDbContext : DbContext
{
    private readonly ICurrentTenantContext _tenantContext;

    public FidelHubDbContext(DbContextOptions<FidelHubDbContext> options, ICurrentTenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Platform Schema
        modelBuilder.Entity<Tenant>(b =>
        {
            b.ToTable("tenants", "platform");
            b.HasKey(t => t.Id);
            b.Property(t => t.Name).HasMaxLength(200).IsRequired();
            b.Property(t => t.Code).HasMaxLength(50).IsRequired();
            b.HasIndex(t => t.Code).IsUnique();
        });

        // School Schema
        modelBuilder.Entity<Branch>(b =>
        {
            b.ToTable("branches", "school");
            b.HasKey(br => br.Id);
            b.Property(br => br.TenantId).IsRequired();
            b.Property(br => br.Name).HasMaxLength(150).IsRequired();
            b.Property(br => br.Code).HasMaxLength(50).IsRequired();
            b.HasIndex(br => new { br.TenantId, br.Code }).IsUnique();
        });

        // Identity Schema
        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("users", "identity");
            b.HasKey(u => u.Id);
            b.Property(u => u.TenantId).IsRequired();
            b.Property(u => u.Email).HasMaxLength(255).IsRequired();
            b.Property(u => u.FullName).HasMaxLength(200).IsRequired();
            b.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
        });

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("roles", "identity");
            b.HasKey(r => r.Id);
            b.Property(r => r.Code).HasMaxLength(50).IsRequired();
            b.HasIndex(r => r.Code).IsUnique();
        });

        // Integration Schema
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("outbox_messages", "integration");
            b.HasKey(m => m.Id);
            b.Property(m => m.TenantId).IsRequired();
            b.Property(m => m.EventType).HasMaxLength(150).IsRequired();
            b.Property(m => m.Payload).IsRequired();
        });

        // Defense-in-depth: Global Query Filter for ITenantScoped entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenantScoped.TenantId));
                var tenantIdProperty = Expression.Property(Expression.Constant(_tenantContext), nameof(ICurrentTenantContext.TenantId));
                var nullableTenantId = Expression.Convert(property, typeof(Guid?));
                var compare = Expression.Equal(nullableTenantId, tenantIdProperty);
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-assign audit stamps if auditable
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditableEntity.CreatedAtUtc)).CurrentValue = DateTime.UtcNow;
                if (_tenantContext.UserId.HasValue)
                {
                    entry.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = _tenantContext.UserId.Value;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditableEntity.UpdatedAtUtc)).CurrentValue = DateTime.UtcNow;
                if (_tenantContext.UserId.HasValue)
                {
                    entry.Property(nameof(IAuditableEntity.UpdatedBy)).CurrentValue = _tenantContext.UserId.Value;
                }
            }
        }

        // Auto-assign tenant_id if adding ITenantScoped and not set
        foreach (var entry in ChangeTracker.Entries<ITenantScoped>())
        {
            if (entry.State == EntityState.Added && _tenantContext.TenantId.HasValue)
            {
                var tenantProp = entry.Property(nameof(ITenantScoped.TenantId));
                if (tenantProp.CurrentValue == null || (Guid)tenantProp.CurrentValue == Guid.Empty)
                {
                    tenantProp.CurrentValue = _tenantContext.TenantId.Value;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
