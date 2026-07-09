using Domain.BoundedContext.DocumentManagement;
using Domain.BoundedContext.Properties;
using Domain.BoundedContext.People;
using Domain.BoundedContext.Tenancy;
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
        modelBuilder.ApplyConfiguration(new OwnerConfig());
        modelBuilder.ApplyConfiguration(new GuestConfig());
        modelBuilder.ApplyConfiguration(new ManagementCompanyConfig());
    }
    public DbSet<PhysicalStructureAgg> PhysicalStructure { get; set; }
    public DbSet<DocumentAgg> Document { get; set; }
    public DbSet<OwnerAgg> Owner { get; set; }
    public DbSet<GuestAgg> Guest { get; set; }
    public DbSet<ManagementCompanyAgg> ManagementCompany { get; set; }
}
