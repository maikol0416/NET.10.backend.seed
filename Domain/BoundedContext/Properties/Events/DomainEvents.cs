using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Domain.Properties.Events;

public record ProductCreatedDomainEvent(Guid ProductId, string Name) : DomainEvent, IDomainEvent
{
    
}

public record ProductPublishedDomainEvent(Guid ProductId) :DomainEvent, IDomainEvent
{

}
