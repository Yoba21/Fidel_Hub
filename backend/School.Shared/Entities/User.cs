using School.Shared.Primitives;
using School.Shared.Tenancy;

namespace School.Shared.Entities;

public class User : Entity, ITenantScoped
{
    public Guid TenantId { get; private set; }
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool EmailVerified { get; private set; }
    public bool TwoFactorEnabled { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private User() { } // EF Core

    public static User Create(Guid id, Guid tenantId, string email, string passwordHash, string fullName, string? phoneNumber = null)
    {
        return new User
        {
            Id = id,
            TenantId = tenantId,
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}

public class Role : Entity
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }

    private Role() { } // EF Core

    public static Role Create(Guid id, string code, string name, string? description = null)
    {
        return new Role
        {
            Id = id,
            Code = code.ToUpperInvariant(),
            Name = name,
            Description = description
        };
    }
}
