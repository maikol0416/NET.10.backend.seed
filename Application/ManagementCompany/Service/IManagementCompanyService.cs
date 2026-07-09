using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Tenancy;

namespace Application.Service;

public interface IManagementCompanyService : IApplicationService<ManagementCompanyAgg, ManagementCompanyDto>
{
}
