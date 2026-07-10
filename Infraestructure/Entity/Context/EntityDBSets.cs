using Domain.BoundedContext.DocumentManagement;
using Domain.BoundedContext.Properties;
using Domain.BoundedContext.People;
using Domain.BoundedContext.Tenancy;
using Domain.Ports;
using Domain.Ports.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Entity;

public abstract class EntityDBSets : DbContext
{
    private readonly ICurrentUserService _currentUser;

    public EntityDBSets(DbContextOptions options, ICurrentUserService currentUser) : base(options)
    {
        _currentUser = currentUser;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PhysicalStructureConfig());
        modelBuilder.ApplyConfiguration(new DocumentConfig());
        modelBuilder.ApplyConfiguration(new OwnerConfig());
        modelBuilder.ApplyConfiguration(new GuestConfig());
        modelBuilder.ApplyConfiguration(new ManagementCompanyConfig());

        // Multi-tenant: un usuario normal solo ve las estructuras de su propia empresa;
        // un Administrator (usuario de plataforma) las ve todas. Solo PhysicalStructure
        // tiene CompanyId por ahora — el resto de aggregates queda sin filtrar.
        modelBuilder.Entity<PhysicalStructureAgg>()
            .HasQueryFilter(p => _currentUser.IsPlatformAdministrator || p.CompanyId == _currentUser.CompanyId);
    }
    public DbSet<PhysicalStructureAgg> PhysicalStructure { get; set; }
    public DbSet<DocumentAgg> Document { get; set; }
    public DbSet<OwnerAgg> Owner { get; set; }
    public DbSet<GuestAgg> Guest { get; set; }
    public DbSet<ManagementCompanyAgg> ManagementCompany { get; set; }
}
