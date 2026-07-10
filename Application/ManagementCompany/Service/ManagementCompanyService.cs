using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Tenancy;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class ManagementCompanyService
    : ApplicationService<ManagementCompanyAgg, ManagementCompanyDto>, IManagementCompanyService
{
    public ManagementCompanyService(IManagementCompanyRepository repository, ICurrentUserService currentUser) : base(repository, currentUser)
    {
        CreateMapperExpresion<ManagementCompanyAgg, ManagementCompanyDto>(cnf =>
        {
            ManagementCompanyMapper.Expresion(cnf);
        });
    }
}
