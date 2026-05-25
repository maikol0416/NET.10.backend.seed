using Domain.Ports.Repository.Base;
using Domain.BoundedContext.DocumentManagement;

namespace Domain.Ports;

/// <summary>
/// Contrato de repositorio de solo lectura para el agregado <see cref="DocumentAgg"/>.
/// Pertenece al Dominio (Ports) — la implementación reside en la capa de Infraestructura.
/// </summary>
public interface IDocumentReadOnlyRepository : IBaseReadOnlyRepository<DocumentAgg>
{
}
