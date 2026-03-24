namespace Domain.Ports.Repository.Base;

public interface IBaseRepository<T>
where T : class, new()
{
    Task<T> CreateAsync(T ent);
}
