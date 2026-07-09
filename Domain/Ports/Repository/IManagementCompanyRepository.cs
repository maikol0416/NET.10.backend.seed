using Domain.Ports.Repository.Base;
using Domain.BoundedContext.Tenancy;

namespace Domain.Ports;

public interface IManagementCompanyRepository : IBaseRepository<ManagementCompanyAgg>
{
}
