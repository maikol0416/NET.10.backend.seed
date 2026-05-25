using Domain.Ports.Repository.Base;
using Domain.BoundedContext.Properties;

namespace Domain.Ports;

/// <summary>
/// Contrato de repositorio de solo lectura para el agregado <see cref="PhysicalStructureAgg"/>.
/// Pertenece al Dominio (Ports) — la implementación reside en la capa de Infraestructura.
/// </summary>
public interface IPhysicalStructureReadOnlyRepository : IBaseReadOnlyRepository<PhysicalStructureAgg>
{
}
