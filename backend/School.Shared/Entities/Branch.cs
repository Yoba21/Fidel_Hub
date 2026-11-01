using School.Shared.Primitives;
using School.Shared.Tenancy;

namespace School.Shared.Entities;

public class Branch : Entity, ITenantScoped
{
    public Guid TenantId { get; private set; }
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string City { get; private set; } = "Addis Ababa";
    public string? Subcity { get; private set; }
    public string? Woreda { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsMainBranch { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Branch() { } // EF Core

    public static Branch Create(Guid id, Guid tenantId, string name, string code, bool isMainBranch = false, string city = "Addis Ababa", string? subcity = null, string? woreda = null, string? phone = null, string? email = null)
    {
        return new Branch
        {
            Id = id,
            TenantId = tenantId,
            Name = name,
            Code = code.ToUpperInvariant(),
            IsMainBranch = isMainBranch,
            City = city,
            Subcity = subcity,
            Woreda = woreda,
            Phone = phone,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
