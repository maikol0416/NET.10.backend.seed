using Application.Base;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;
using Domain.Ports;

namespace Application.Service;

/// <summary>
/// Servicio de solo lectura para <see cref="DocumentAgg"/> (CQRS — Query side).
/// Hereda <see cref="ApplicationReadOnlyService{ENT,DTO}"/> que usa
/// <see cref="IDocumentReadOnlyRepository"/> conectado al <c>EntityReadOnlyDbContext</c> (NoTracking).
///
/// Registra el mismo mapper que <see cref="DocumentService"/> para mantener
/// consistencia en la proyección Agg → DTO.
/// </summary>
public class DocumentReadOnlyService
    : ApplicationReadOnlyService<DocumentAgg, DocumentDto>,
      IDocumentReadOnlyService
{
    public DocumentReadOnlyService(IDocumentReadOnlyRepository repository)
        : base(repository)
    {
        CreateMapperExpresion<DocumentAgg, DocumentDto>(cnf =>
        {
            DocumentMapper.Expresion(cnf);
        });
    }
}
