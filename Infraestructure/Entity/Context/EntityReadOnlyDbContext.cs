using Domain.Ports;
using Domain.Ports.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;
public class EntityReadOnlyDbContext : EntityDBSets, IEntityReadOnlyDbContext
{
    public EntityReadOnlyDbContext(DbContextOptions<EntityReadOnlyDbContext> options, ICurrentUserService currentUser)
        : base(options, currentUser)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
    }
}

