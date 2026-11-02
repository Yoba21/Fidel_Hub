namespace School.Shared.Primitives;

/// <summary>
/// Base class for all domain entities with a Guid identifier.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    private readonly List<object> _domainEvents = new();

    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Generic base entity supporting custom identifier types.
/// </summary>
public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;

    private readonly List<object> _domainEvents = new();

    [System.Text.Json.Serialization.JsonIgnore]
    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
