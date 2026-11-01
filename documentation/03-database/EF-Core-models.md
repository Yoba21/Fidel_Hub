# ENTITY FRAMEWORK CORE MODELS & CONFIGURATION

## Fidel Hub — ORM Mapping & Tenancy Interceptors

| Document Information | Details |
| :--- | :--- |
| **Document** | EF Core Models & Mapping Guide |
| **Version** | 1.0-DRAFT |

---

## 1. Global Query Filters & Tenancy
EF Core maps composite keys `(TenantId, Id)` and integrates with PostgreSQL RLS:

```csharp
public abstract class BaseEntityTypeConfiguration<TEntity, TId> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity<TId>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => new { e.TenantId, e.Id });
        builder.Property(e => e.CreatedAtUtc).IsRequired();
        builder.Property(e => e.CreatedBy).HasMaxLength(100);
    }
}
```

---

## 2. PostgreSQL RLS Session Interceptor
Before queries execute on an opened connection, EF Core executes the tenant session command:

```csharp
public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly ICurrentTenantContext _tenantContext;

    public TenantConnectionInterceptor(ICurrentTenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpenedAsync(
        DbConnection connection,
        ConnectionEndEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = default)
    {
        if (_tenantContext.TenantId.HasValue)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET LOCAL app.current_tenant_id = '{_tenantContext.TenantId.Value}';";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        return await base.ConnectionOpenedAsync(connection, eventData, result, cancellationToken);
    }
}
```
