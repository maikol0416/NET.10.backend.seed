using System.Linq.Expressions;

namespace Application.Base;

public interface IApplicationReadOnlyService<ENT, DTO>
    where ENT : class, new()
    where DTO : class, new()
{
    Task<DTO?> GetByIdAsync(Guid id);

    Task<IEnumerable<DTO>> GetAllAsync();

    Task<IEnumerable<DTO>> FindAsync(Expression<Func<ENT, bool>> predicate);

    Task<bool> ExistsAsync(Expression<Func<ENT, bool>> predicate);

    Task<Domain.DomainShared.PaginatedList<DTO>> GetPaginatedAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Paginado con filtro — transversal a cualquier módulo. El predicado se expresa en
    /// términos de la entidad de dominio (ENT), no del DTO.
    /// </summary>
    Task<Domain.DomainShared.PaginatedList<DTO>> GetPaginatedAsync(int pageNumber, int pageSize, Expression<Func<ENT, bool>> predicate);
}
