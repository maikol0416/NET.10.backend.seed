using Domain.Ports;
using Domain.Ports.Repository.Base;
using Microsoft.EntityFrameworkCore;
using Util.Common;

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

    public async Task CreateListAsync(List<T> entities)
    {
        await entity.AddRangeAsync(entities);
        await MainContext.SaveChangesAsync();
    }

    public virtual async Task<T> UpdateAsync(T ent)
    {
        entity.Update(ent);
        await MainContext.SaveChangesAsync();
        return ent;
    }

    public async Task<bool> DeleteEntity(int id)
    {
        bool returnDelete = false;

        T obj = await GetById(id);

        if (obj != null)
        {
            entity.Remove(obj);
            returnDelete = await MainContext.SaveChangesAsync()>0;
        }

        return returnDelete;
    }

    public virtual async Task<T> GetById(int id)
    {
        return await entity.FindAsync(id)?? new T();
    }
}
