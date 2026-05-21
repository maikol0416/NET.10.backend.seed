using Application.Base;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;
using Domain.Ports;

namespace Application.Service;

public class DocumentService:ApplicationService<DocumentAgg, DocumentDto>, IDocumentService
{
    public DocumentService(IDocumentRepository repository) : base(repository)
    {
        CreateMapperExpresion<DocumentAgg, DocumentDto>(cnf =>
        {
            DocumentMapper.Expresion(cnf);
        });
    }
}
