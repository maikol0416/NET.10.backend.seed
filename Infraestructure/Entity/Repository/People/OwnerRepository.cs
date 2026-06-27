using Domain.Ports;
using Domain.BoundedContext.People;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.People;

public class OwnerRepository : BaseRepositiry<OwnerAgg>, IOwnerRepository
{
    public OwnerRepository(IEntityDbContext entityDbContext) : base(entityDbContext)
    {
    }
}
