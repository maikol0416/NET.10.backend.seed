using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;
public class EntityReadOnlyDbContext : EntityDBSets, IEntityReadOnlyDbContext
{
    public EntityReadOnlyDbContext(DbContextOptions<EntityReadOnlyDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
    }
}

