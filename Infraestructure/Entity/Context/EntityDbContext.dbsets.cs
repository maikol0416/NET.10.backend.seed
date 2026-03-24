using Domain.BoundedContext.Properties;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infraestructure.Entity;

public partial class EntityDbContext : DbContext,IEntityDbContext
{   
    public DbSet<PhysicalStructureAgg> PhysicalStructure { get; set; }
}
