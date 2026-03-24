using Domain.Ports;
using Domain.Ports.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository.Shared;

public class BaseRepositiry<T> : IBaseRepository<T>
    where T : class, new()
{
    public IEntityDbContext MainContext { get; set; }
    protected DbSet<T> entity;

    public BaseRepositiry(IEntityDbContext mainContext)
    {
        MainContext = mainContext;
        entity = MainContext.Set<T>();
    }

    public virtual async Task<T> CreateAsync(T ent)
    {
        await entity.AddAsync(ent);
        await MainContext.SaveChangesAsync();
        return ent;
    }
}
