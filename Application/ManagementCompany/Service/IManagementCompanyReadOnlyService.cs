using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Tenancy;

namespace Application.Service;

/// <summary>
/// Contrato del servicio de solo lectura para <see cref="ManagementCompanyAgg"/>.
/// Hereda todas las operaciones de <see cref="IApplicationReadOnlyService{ENT,DTO}"/>:
/// GetByIdAsync, GetAllAsync, FindAsync, ExistsAsync.
/// </summary>
public interface IManagementCompanyReadOnlyService
    : IApplicationReadOnlyService<ManagementCompanyAgg, ManagementCompanyDto>
{
}
