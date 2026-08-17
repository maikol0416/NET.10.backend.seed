using System.Linq.Expressions;
using Domain.Ports.Repository.Base;

namespace Application.Base;

public abstract partial class ApplicationReadOnlyService<ENT, DTO>
    : ApplicationServiceMapper<ENT, DTO>, IApplicationReadOnlyService<ENT, DTO>
    where ENT : class, new()
    where DTO : class, new()
{
    protected IBaseReadOnlyRepository<ENT> ReadOnlyRepository { get; }

    protected ApplicationReadOnlyService(IBaseReadOnlyRepository<ENT> readOnlyRepository)
    {
        ReadOnlyRepository = readOnlyRepository;
    }

    /// <inheritdoc/>
    public virtual async Task<DTO?> GetByIdAsync(Guid id)
    {
        var entity = await ReadOnlyRepository.GetByIdAsync(id);
        return entity is null ? null : MapToDTO(entity);
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<DTO>> GetAllAsync()
    {
        var entities = await ReadOnlyRepository.GetAllAsync();
        return MapLstToDTO(entities.ToList());
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<DTO>> FindAsync(Expression<Func<ENT, bool>> predicate)
    {
        var entities = await ReadOnlyRepository.FindAsync(predicate);
        return MapLstToDTO(entities.ToList());
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(Expression<Func<ENT, bool>> predicate)
    {
        return await ReadOnlyRepository.ExistsAsync(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<Domain.DomainShared.PaginatedList<DTO>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        var result = await ReadOnlyRepository.GetPaginatedAsync(pageNumber, pageSize);
        var dtoList = MapLstToDTO(result.Items.ToList());
        return new Domain.DomainShared.PaginatedList<DTO>(dtoList, result.TotalCount, result.PageNumber, result.PageSize);
    }

    /// <inheritdoc/>
    public virtual async Task<Domain.DomainShared.PaginatedList<DTO>> GetPaginatedAsync(int pageNumber, int pageSize, Expression<Func<ENT, bool>> predicate)
    {
        var result = await ReadOnlyRepository.GetPaginatedAsync(pageNumber, pageSize, predicate);
        var dtoList = MapLstToDTO(result.Items.ToList());
        return new Domain.DomainShared.PaginatedList<DTO>(dtoList, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
