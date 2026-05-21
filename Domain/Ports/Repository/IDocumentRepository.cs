using Domain.Ports.Repository.Base;
using Domain.BoundedContext.DocumentManagement;

namespace Domain.Ports;

public interface IDocumentRepository : IBaseRepository<DocumentAgg>
{
}
