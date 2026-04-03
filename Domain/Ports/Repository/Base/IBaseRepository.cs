namespace Domain.Ports.Repository.Base;

public interface IBaseRepository<T>
where T : class, new()
{
    Task<T> CreateAsync(T ent);
    Task CreateListAsync(List<T> entities);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteEntity(int id);
    Task<T> GetById(int id);

}
