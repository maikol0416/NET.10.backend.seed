using Microsoft.EntityFrameworkCore;

namespace Domain.Ports;

/// <summary>
/// Puerto de lectura para el contexto de base de datos.
/// Solo expone consultas — no permite persistir cambios (CQRS: Query side).
/// </summary>
public interface IEntityReadOnlyDbContext
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
