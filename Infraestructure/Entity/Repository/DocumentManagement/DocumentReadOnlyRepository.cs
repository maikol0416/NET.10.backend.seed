using Domain.Ports;
using Domain.BoundedContext.DocumentManagement;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.DocumentManagement;

/// <summary>
/// Implementación del repositorio de solo lectura para <see cref="DocumentAgg"/>.
/// Usa <see cref="BaseReadOnlyRepository{T}"/> conectado al <c>EntityReadOnlyDbContext</c>
/// (NoTracking) para optimizar las consultas del lado Query del CQRS.
/// </summary>
public class DocumentReadOnlyRepository
    : BaseReadOnlyRepository<DocumentAgg>, IDocumentReadOnlyRepository
{
    public DocumentReadOnlyRepository(IEntityReadOnlyDbContext readOnlyContext)
        : base(readOnlyContext)
    {
    }
}
