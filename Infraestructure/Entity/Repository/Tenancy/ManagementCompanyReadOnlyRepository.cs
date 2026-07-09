using Domain.Ports;
using Domain.BoundedContext.Tenancy;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.Tenancy;

public class ManagementCompanyReadOnlyRepository
    : BaseReadOnlyRepository<ManagementCompanyAgg>, IManagementCompanyReadOnlyRepository
{
    public ManagementCompanyReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext)
        : base(readOnlyContext)
    {
    }
}
