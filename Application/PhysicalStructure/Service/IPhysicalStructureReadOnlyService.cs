using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;

namespace Application.Service;

/// <summary>
/// Contrato del servicio de solo lectura para <see cref="PhysicalStructureAgg"/>.
/// Hereda todas las operaciones de <see cref="IApplicationReadOnlyService{ENT,DTO}"/>:
/// GetByIdAsync, GetAllAsync, FindAsync, ExistsAsync.
/// </summary>
public interface IPhysicalStructureReadOnlyService
    : IApplicationReadOnlyService<PhysicalStructureAgg, PhysicalStructureDto>
{
}
