namespace School.Shared.Tenancy;

/// <summary>
/// Marks an entity as owned by a specific SaaS school tenant.
/// Automatically enforced via PostgreSQL Row-Level Security (RLS) and EF Core global query filters.
/// </summary>
public interface ITenantScoped
{
    Guid TenantId { get; }
}

/// <summary>
/// Marks an entity as owned by a specific physical branch campus within a school tenant.
/// </summary>
public interface IBranchScoped : ITenantScoped
{
    Guid BranchId { get; }
}

/// <summary>
/// Tracks creation and modification audit stamps.
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; }
    Guid? CreatedBy { get; }
    DateTime? UpdatedAtUtc { get; }
    Guid? UpdatedBy { get; }
}
