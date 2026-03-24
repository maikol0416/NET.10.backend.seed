using Domain.Ports;
using Domain.BoundedContext.Properties;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.Properties;

public class PhysicalStructureRepository: BaseRepositiry<PhysicalStructureAgg>, IPhysicalStructureRepository
{
    public PhysicalStructureRepository(IEntityDbContext entityDbContext):
    base(entityDbContext)
    {
        
    }
}
