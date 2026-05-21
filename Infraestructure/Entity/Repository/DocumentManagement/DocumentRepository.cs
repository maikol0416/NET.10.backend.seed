using Domain.Ports;
using Domain.BoundedContext.DocumentManagement;
using Infraestructure.Repository.Shared;

namespace Infraestructure.Repository.DocumentManagement;

public class DocumentRepository : BaseRepositiry<DocumentAgg>, IDocumentRepository
{
    public DocumentRepository(IEntityDbContext entityDbContext)
        : base(entityDbContext)
    {
    }
}
