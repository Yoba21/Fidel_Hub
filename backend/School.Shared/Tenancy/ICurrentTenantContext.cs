namespace School.Shared.Tenancy;

/// <summary>
/// Scoped tenant and branch context resolved per HTTP request or background unit of work.
/// </summary>
public interface ICurrentTenantContext
{
    Guid? TenantId { get; }
    string? TenantCode { get; }
    Guid? BranchId { get; }
    Guid? UserId { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsResolved { get; }

    bool HasPermission(string permission);
    bool IsInRole(string role);

    void SetTenant(Guid tenantId, string? tenantCode = null);
    void SetContext(Guid tenantId, Guid? branchId, Guid? userId, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? tenantCode = null);
}

public class CurrentTenantContext : ICurrentTenantContext
{
    private readonly HashSet<string> _roles = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);
    private IReadOnlyList<string> _cachedRoles = Array.Empty<string>();
    private IReadOnlyList<string> _cachedPermissions = Array.Empty<string>();

    public Guid? TenantId { get; private set; }
    public string? TenantCode { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? UserId { get; private set; }
    
    public IReadOnlyList<string> Roles => _cachedRoles;
    public IReadOnlyList<string> Permissions => _cachedPermissions;
    
    public bool IsResolved => TenantId.HasValue;

    public bool HasPermission(string permission)
    {
        if (string.IsNullOrWhiteSpace(permission)) return false;
        return _permissions.Contains(permission) || _roles.Contains("PLATFORM_SUPERADMIN");
    }

    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role)) return false;
        return _roles.Contains(role);
    }

    public void SetTenant(Guid tenantId, string? tenantCode = null)
    {
        TenantId = tenantId;
        TenantCode = tenantCode;
        _roles.Clear();
        _cachedRoles = Array.Empty<string>();
        _permissions.Clear();
        _cachedPermissions = Array.Empty<string>();
    }

    public void SetContext(Guid tenantId, Guid? branchId, Guid? userId, IEnumerable<string>? roles = null, IEnumerable<string>? permissions = null, string? tenantCode = null)
    {
        TenantId = tenantId;
        BranchId = branchId;
        UserId = userId;
        TenantCode = tenantCode;

        _roles.Clear();
        if (roles != null)
        {
            foreach (var r in roles) _roles.Add(r);
        }
        _cachedRoles = _roles.ToArray();

        _permissions.Clear();
        if (permissions != null)
        {
            foreach (var p in permissions) _permissions.Add(p);
        }
        _cachedPermissions = _permissions.ToArray();
    }
}
