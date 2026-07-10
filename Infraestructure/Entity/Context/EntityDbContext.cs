using Domain.Ports;
using Domain.Ports.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

public partial class EntityDbContext : EntityDBSets, IEntityDbContext
{
    public EntityDbContext(DbContextOptions<EntityDbContext> options, ICurrentUserService currentUser)
        : base(options, currentUser)
    {

    }
}
