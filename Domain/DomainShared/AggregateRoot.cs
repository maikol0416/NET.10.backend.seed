using Domain.Ports.Events.Properties;

namespace Domain.DomainShared;

public abstract class AggregateRoot : Entity
{
    public AggregateRoot()
    {
        
    }
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected abstract void ExcecuteDomainInvariants();
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
