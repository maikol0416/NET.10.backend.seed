using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

public partial class EntityDbContext : DbContext,IEntityDbContext
{
    public EntityDbContext(DbContextOptions<EntityDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PhysicalStructureConfig());
    }

}
