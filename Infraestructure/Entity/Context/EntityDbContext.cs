using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

public partial class EntityDbContext : EntityDBSets, IEntityDbContext
{
    public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options)
    {

    }
}
