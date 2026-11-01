using School.Shared.Primitives;

namespace School.Shared.Entities;

public class Tenant : Entity
{
    public string Name { get; private set; } = default!;
    public string Code { get; private set; } = default!;
    public string? LegalName { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string Status { get; private set; } = "ACTIVE"; // PENDING, ACTIVE, SUSPENDED, CLOSED
    public string DefaultCurrency { get; private set; } = "ETB";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    private Tenant() { } // EF Core

    public static Tenant Create(Guid id, string name, string code, string? legalName = null, string? email = null, string? phone = null, string defaultCurrency = "ETB")
    {
        return new Tenant
        {
            Id = id,
            Name = name,
            Code = code.ToLowerInvariant(),
            LegalName = legalName,
            Email = email,
            Phone = phone,
            Status = "ACTIVE",
            DefaultCurrency = defaultCurrency,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateStatus(string newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}
