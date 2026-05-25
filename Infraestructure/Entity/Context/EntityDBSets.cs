using Domain.BoundedContext.DocumentManagement;
using Domain.BoundedContext.Properties;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

public abstract class EntityDBSets : DbContext
{   
    public EntityDBSets(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PhysicalStructureConfig());
        modelBuilder.ApplyConfiguration(new DocumentConfig());
    }
    public DbSet<PhysicalStructureAgg> PhysicalStructure { get; set; }
    public DbSet<DocumentAgg> Document { get; set; }
}
