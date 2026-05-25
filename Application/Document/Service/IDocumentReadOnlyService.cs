using Application.Base;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;

namespace Application.Service;

/// <summary>
/// Contrato del servicio de solo lectura para <see cref="DocumentAgg"/>.
/// Hereda todas las operaciones de <see cref="IApplicationReadOnlyService{ENT,DTO}"/>:
/// GetByIdAsync, GetAllAsync, FindAsync, ExistsAsync.
/// </summary>
public interface IDocumentReadOnlyService
    : IApplicationReadOnlyService<DocumentAgg, DocumentDto>
{
}
