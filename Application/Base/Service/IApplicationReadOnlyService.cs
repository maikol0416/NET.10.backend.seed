using System.Linq.Expressions;

namespace Application.Base;

public interface IApplicationReadOnlyService<ENT, DTO>
    where ENT : class, new()
    where DTO : class, new()
{
    Task<DTO?> GetByIdAsync(int id);

    Task<IEnumerable<DTO>> GetAllAsync();

    Task<IEnumerable<DTO>> FindAsync(Expression<Func<ENT, bool>> predicate);

    Task<bool> ExistsAsync(Expression<Func<ENT, bool>> predicate);

    Task<Domain.DomainShared.PaginatedList<DTO>> GetPaginatedAsync(int pageNumber, int pageSize);
}
