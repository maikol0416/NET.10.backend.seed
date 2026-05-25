using Domain.Ports;
using Domain.BoundedContext.Properties;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.Properties;

/// <summary>
/// Implementación del repositorio de solo lectura para <see cref="PhysicalStructureAgg"/>.
/// Usa <see cref="BaseReadOnlyRepository{T}"/> conectado al <c>EntityReadOnlyDbContext</c>
/// (NoTracking) para optimizar las consultas del lado Query del CQRS.
/// </summary>
public class PhysicalStructureReadOnlyRepository
    : BaseReadOnlyRepository<PhysicalStructureAgg>, IPhysicalStructureReadOnlyRepository
{
    public PhysicalStructureReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext)
        : base(readOnlyContext)
    {
    }
}
