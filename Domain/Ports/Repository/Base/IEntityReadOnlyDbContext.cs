using Microsoft.EntityFrameworkCore;

namespace Domain.Ports;

/// <summary>
/// Puerto de lectura para el contexto de base de datos (CQRS — Query side).
/// Solo expone <see cref="Set{TEntity}"/> para realizar consultas.
/// No hereda de <see cref="IEntityDbContext"/> para garantizar que no se pueda
/// persistir ningún cambio a través de este contrato.
/// </summary>
public interface IEntityReadOnlyDbContext
{
    /// <summary>
    /// Accede al conjunto de entidades del tipo indicado para realizar consultas.
    /// El contexto subyacente opera en modo NoTracking.
    /// </summary>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
}
