using System.Linq.Expressions;
using Domain.Ports;
using Domain.Ports.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository.Shared;

public class BaseReadOnlyRepository<T> : IBaseReadOnlyRepository<T>
    where T : class, new()
{
    protected readonly IEntityReadOnlyDbContext ReadContext;
    protected readonly DbSet<T> Entity;

    public BaseReadOnlyRepository(IEntityReadOnlyDbContext readContext)
    {
        ReadContext = readContext;
        Entity = ReadContext.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await Entity.FindAsync(id);
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await Entity.ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await Entity.Where(predicate).ToListAsync();
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await Entity.AnyAsync(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<Domain.DomainShared.PaginatedList<T>> GetPaginatedAsync(int pageNumber, int pageSize)
    {
        var totalCount = await Entity.CountAsync();
        var items = await Entity.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new Domain.DomainShared.PaginatedList<T>(items, totalCount, pageNumber, pageSize);
    }
}
