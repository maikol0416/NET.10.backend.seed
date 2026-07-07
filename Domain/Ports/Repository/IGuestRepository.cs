using Domain.Ports.Repository.Base;
using Domain.BoundedContext.People;

namespace Domain.Ports;

public interface IGuestRepository : IBaseRepository<GuestAgg>
{
}
