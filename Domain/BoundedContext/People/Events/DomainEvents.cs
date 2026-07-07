using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Domain.BoundedContext.People.Events;

public record OwnerCreatedEvent(Guid OwnerId) : DomainEvent, IDomainEvent;

public record GuestCreatedEvent(Guid GuestId) : DomainEvent, IDomainEvent;
