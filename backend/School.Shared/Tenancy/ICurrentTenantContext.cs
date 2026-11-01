namespace School.Shared.Tenancy;

/// <summary>
/// Scoped tenant context resolved per HTTP request or background unit of work.
/// </summary>
public interface ICurrentTenantContext
{
    Guid? TenantId { get; }
    string? Subdomain { get; }
    bool IsResolved { get; }
    void SetTenant(Guid tenantId, string? subdomain = null);
}

public class CurrentTenantContext : ICurrentTenantContext
{
    public Guid? TenantId { get; private set; }
    public string? Subdomain { get; private set; }
    public bool IsResolved => TenantId.HasValue;

    public void SetTenant(Guid tenantId, string? subdomain = null)
    {
        TenantId = tenantId;
        Subdomain = subdomain;
    }
}
