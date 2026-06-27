using Domain.Ports;
using Domain.BoundedContext.People;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.People;

public class OwnerReadOnlyRepository : BaseReadOnlyRepository<OwnerAgg>, IOwnerReadOnlyRepository
{
    public OwnerReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext) : base(readOnlyContext)
    {
    }
}
