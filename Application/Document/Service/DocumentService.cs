using Application.Base;
using Application.Dto;
using Domain.BoundedContext.DocumentManagement;
using Domain.Ports;
using Domain.Ports.Identity;

namespace Application.Service;

public class DocumentService:ApplicationService<DocumentAgg, DocumentDto>, IDocumentService
{
    public DocumentService(IDocumentRepository repository, ICurrentUserService currentUser) : base(repository, currentUser)
    {
        CreateMapperExpresion<DocumentAgg, DocumentDto>(cnf =>
        {
            DocumentMapper.Expresion(cnf);
        });
    }
}
