using Domain.Ports;
using Domain.BoundedContext.Tenancy;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.Tenancy;

public class ManagementCompanyRepository : BaseRepositiry<ManagementCompanyAgg>, IManagementCompanyRepository
{
    public ManagementCompanyRepository(IEntityDbContext entityDbContext)
        : base(entityDbContext)
    {
    }
}
