using Application.Base;
using Application.Dto;
using Domain.BoundedContext.Properties;
using Domain.Ports;

namespace Application.Service;

/// <summary>
/// Servicio de solo lectura para <see cref="PhysicalStructureAgg"/> (CQRS — Query side).
/// Hereda <see cref="ApplicationReadOnlyService{ENT,DTO}"/> que usa
/// <see cref="IPhysicalStructureReadOnlyRepository"/> conectado al <c>EntityReadOnlyDbContext</c> (NoTracking).
///
/// Registra el mismo mapper que <see cref="PhysicalStructureService"/> para mantener
/// consistencia en la proyección Agg → DTO.
/// </summary>
public class PhysicalStructureReadOnlyService
    : ApplicationReadOnlyService<PhysicalStructureAgg, PhysicalStructureDto>,
      IPhysicalStructureReadOnlyService
{
    public PhysicalStructureReadOnlyService(IPhysicalStructureReadOnlyRepository repository)
        : base(repository)
    {
        CreateMapperExpresion<PhysicalStructureAgg, PhysicalStructureDto>(cnf =>
        {
            PhysicalStructureMapper.Expresion(cnf);
        });
    }
}
