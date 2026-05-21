using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

/// <summary>
/// Contexto de EF Core dedicado exclusivamente a lecturas (CQRS — Query side).
/// Comparte la misma cadena de conexión que <see cref="EntityDbContext"/> pero:
///   - No expone SaveChanges ni SaveChangesAsync.
///   - Habilita QueryTrackingBehavior.NoTrackingWithIdentityResolution por defecto
///     para evitar overhead del ChangeTracker en consultas.
/// </summary>
public class EntityReadOnlyDbContext : DbContext, IEntityReadOnlyDbContext
{
    public EntityReadOnlyDbContext(DbContextOptions<EntityReadOnlyDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PhysicalStructureConfig());
        modelBuilder.ApplyConfiguration(new DocumentConfig());
    }
}
