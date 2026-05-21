using Application.Base;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;

namespace Application.Service;

public interface IDocumentService : IApplicationService<DocumentAgg, DocumentDto>
{
}
