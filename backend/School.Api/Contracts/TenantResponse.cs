namespace School.Api.Contracts;

/// <summary>
/// DTO response for current tenant endpoint, preventing domain entity leaks.
/// </summary>
public sealed record TenantResponse(
    Guid Id,
    string Name,
    string Code,
    string? LegalName,
    string? Email,
    string? Phone,
    string Status,
    string DefaultCurrency,
    DateTime CreatedAt,
    DateTime UpdatedAt);
