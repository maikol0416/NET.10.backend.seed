using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Tenancy;
using Domain.Ports;

namespace Application.Service;

/// <summary>
/// Servicio de solo lectura para <see cref="ManagementCompanyAgg"/> (CQRS — Query side).
/// Hereda <see cref="ApplicationReadOnlyService{ENT,DTO}"/> que usa
/// <see cref="IManagementCompanyReadOnlyRepository"/> conectado al <c>EntityReadOnlyDbContext</c> (NoTracking).
///
/// Registra el mismo mapper que <see cref="ManagementCompanyService"/> para mantener
/// consistencia en la proyección Agg → DTO.
/// </summary>
public class ManagementCompanyReadOnlyService
    : ApplicationReadOnlyService<ManagementCompanyAgg, ManagementCompanyDto>,
      IManagementCompanyReadOnlyService
{
    public ManagementCompanyReadOnlyService(IManagementCompanyReadOnlyRepository repository)
        : base(repository)
    {
        CreateMapperExpresion<ManagementCompanyAgg, ManagementCompanyDto>(cnf =>
        {
            ManagementCompanyMapper.Expresion(cnf);
        });
    }
}
