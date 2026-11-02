using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

    public Guid? CurrentTenantId => _tenantContext.TenantId;

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
            b.HasAlternateKey(br => new { br.TenantId, br.Id });
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
            b.HasAlternateKey(u => new { u.TenantId, u.Id });
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
            b.HasAlternateKey(m => new { m.TenantId, m.Id });
            b.Property(m => m.TenantId).IsRequired();
            b.Property(m => m.EventType).HasMaxLength(150).IsRequired();
            b.Property(m => m.Payload).HasColumnType("jsonb").IsRequired();
            b.Property(m => m.OccurredAt).HasColumnName("occurred_at").IsRequired();
            b.Property(m => m.ProcessedAt).HasColumnName("processed_at");
            b.Property(m => m.RetryCount).HasColumnName("retry_count").IsRequired();
            b.Property(m => m.Error).HasColumnName("error");
        });

        // Defense-in-depth: Parameterized Global Query Filter for ITenantScoped entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantScoped).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ITenantScoped.TenantId));
                var tenantIdProperty = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
                var nullableTenantId = Expression.Convert(property, typeof(Guid?));
                var compare = Expression.Equal(nullableTenantId, tenantIdProperty);
                var lambda = Expression.Lambda(compare, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }

        // Apply snake_case naming conventions across all tables, columns, keys, and indexes
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                var explicitColumn = property.FindAnnotation(RelationalAnnotationNames.ColumnName)?.Value as string;
                if (string.IsNullOrEmpty(explicitColumn))
                {
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (keyName != null) key.SetName(ToSnakeCase(keyName));
            }
            foreach (var fk in entity.GetForeignKeys())
            {
                var fkName = fk.GetConstraintName();
                if (fkName != null) fk.SetConstraintName(ToSnakeCase(fkName));
            }
            foreach (var index in entity.GetIndexes())
            {
                var indexName = index.GetDatabaseName();
                if (indexName != null) index.SetDatabaseName(ToSnakeCase(indexName));
            }
        }
    }

    public override int SaveChanges()
    {
        ApplyAuditAndTenantStamps();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditAndTenantStamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditAndTenantStamps()
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
                if (tenantProp.CurrentValue is not Guid currentGuid || currentGuid == Guid.Empty)
                {
                    tenantProp.CurrentValue = _tenantContext.TenantId.Value;
                }
            }
        }
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
    }
}
