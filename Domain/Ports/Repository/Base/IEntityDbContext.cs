
using Microsoft.EntityFrameworkCore;

namespace Domain.Ports;

public interface IEntityDbContext
{
	DbSet<TEntity> Set<TEntity>() where TEntity : class;
	int SaveChanges();
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
