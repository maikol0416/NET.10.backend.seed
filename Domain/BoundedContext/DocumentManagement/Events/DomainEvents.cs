using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Domain.BoundedContext.DocumentManagement.Events;

// Evento: Document creado
public record DocumentCreatedDomainEvent(Guid DocumentId, string Name)
    : DomainEvent, IDomainEvent
{
}

// public record DocumentUpdatedDomainEvent(Guid DocumentId) : DomainEvent, IDomainEvent { }
