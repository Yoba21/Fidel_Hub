namespace School.Shared.Primitives;

public interface IAuditableEntity
{
    DateTime CreatedAtUtc { get; set; }
    string? CreatedBy { get; set; }
    DateTime? LastModifiedAtUtc { get; set; }
    string? LastModifiedBy { get; set; }
}

public interface ITenantScopedEntity
{
    Guid TenantId { get; set; }
}

public abstract class BaseEntity<TId> : ITenantScopedEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public TId Id { get; protected set; } = default!;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAtUtc { get; set; }
    public string? LastModifiedBy { get; set; }
}
