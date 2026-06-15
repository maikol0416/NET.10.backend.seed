using System.Linq.Expressions;

namespace Domain.Ports.Repository.Base;

/// <summary>
/// Contrato genérico de repositorio para el lado de lectura (CQRS — Query side).
/// Solo expone operaciones de consulta — nunca de escritura.
/// El contrato pertenece al Dominio; la implementación pertenece a la Infraestructura.
/// </summary>
/// <typeparam name="T">Tipo del Aggregate Root o entidad de lectura.</typeparam>
public interface IBaseReadOnlyRepository<T>
    where T : class, new()
{
    /// <summary>Obtiene una entidad por su identificador. Retorna null si no existe.</summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>Retorna todas las entidades activas de la colección.</summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retorna las entidades que satisfacen el predicado indicado.
    /// Útil para consultas con criterio expresado en Lenguaje Ubicuo del repositorio concreto.
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>Indica si existe al menos una entidad que satisface el predicado.</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>Retorna un listado paginado de entidades.</summary>
    Task<Domain.DomainShared.PaginatedList<T>> GetPaginatedAsync(int pageNumber, int pageSize);
}
