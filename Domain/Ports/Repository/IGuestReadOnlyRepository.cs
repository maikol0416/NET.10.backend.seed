using Domain.Ports.Repository.Base;
using Domain.BoundedContext.People;

namespace Domain.Ports;

public interface IGuestReadOnlyRepository : IBaseReadOnlyRepository<GuestAgg>
{
}
