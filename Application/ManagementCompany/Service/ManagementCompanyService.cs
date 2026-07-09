using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Tenancy;
using Domain.Ports;

namespace Application.Service;

public class ManagementCompanyService
    : ApplicationService<ManagementCompanyAgg, ManagementCompanyDto>, IManagementCompanyService
{
    public ManagementCompanyService(IManagementCompanyRepository repository) : base(repository)
    {
        CreateMapperExpresion<ManagementCompanyAgg, ManagementCompanyDto>(cnf =>
        {
            ManagementCompanyMapper.Expresion(cnf);
        });
    }
}
