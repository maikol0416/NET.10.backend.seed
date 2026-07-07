using Domain.Ports;
using Domain.BoundedContext.People;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.People;

public class GuestReadOnlyRepository : BaseReadOnlyRepository<GuestAgg>, IGuestReadOnlyRepository
{
    public GuestReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext) : base(readOnlyContext)
    {
    }
}
