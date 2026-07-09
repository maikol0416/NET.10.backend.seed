using Domain.DomainShared;
using Domain.Ports.Events.Properties;

namespace Domain.BoundedContext.Tenancy.Events;

public record ManagementCompanyCreatedDomainEvent(Guid ManagementCompanyId, string Name) : DomainEvent, IDomainEvent
{
}
