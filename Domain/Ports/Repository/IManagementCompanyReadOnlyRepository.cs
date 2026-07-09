using Domain.Ports.Repository.Base;
using Domain.BoundedContext.Tenancy;

namespace Domain.Ports;

/// <summary>
/// Contrato de repositorio de solo lectura para el agregado <see cref="ManagementCompanyAgg"/>.
/// Pertenece al Dominio (Ports) — la implementación reside en la capa de Infraestructura.
/// </summary>
public interface IManagementCompanyReadOnlyRepository : IBaseReadOnlyRepository<ManagementCompanyAgg>
{
}
