using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.Properties;

namespace Infraestructure.Entity;
public class PhysicalStructureConfig :IEntityTypeConfiguration<PhysicalStructureAgg>
{
    public void Configure(EntityTypeBuilder<PhysicalStructureAgg> builder)
    {
        builder.ToTable("PhysicalStructures");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdateAt)
            .IsRequired(false);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);
        
        builder.Property(p => p.Nit)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(p => p.UnitCount)
            .IsRequired();

        builder.OwnsOne(p => p.Location, locationBuilder =>
        {
            locationBuilder.ToTable("Location");

            locationBuilder.WithOwner().HasForeignKey("PhysicalStructureId");

            locationBuilder.Property<int>("Id");
            locationBuilder.HasKey("Id");

            locationBuilder.Property(l => l.Number)
                .HasColumnName("Number")
                .IsRequired()
                .HasMaxLength(20);

            locationBuilder.Property(l => l.Detail)
                .HasColumnName("Detail")
                .IsRequired()
                .HasMaxLength(200);
            
            locationBuilder.Property(l => l.Country)
                .HasColumnName("Country")
                .IsRequired()
                .HasMaxLength(100);
            
            locationBuilder.Property(l => l.City)
                .HasColumnName("City")
                .IsRequired()
                .HasMaxLength(100);
            
            locationBuilder.Property(l => l.Neighborhood)
                .HasColumnName("Neighborhood")
                .IsRequired()
                .HasMaxLength(100);
        });

        builder.OwnsMany(p => p.CommonsAreas, commonAreaBuilder => 
        {
            commonAreaBuilder.ToTable("CommonArea");
            commonAreaBuilder.WithOwner().HasForeignKey("PhysicalStructureId");
            commonAreaBuilder.Property(t => t.Id).ValueGeneratedNever();
            commonAreaBuilder.HasKey(t => t.Id);

            commonAreaBuilder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(10);

            commonAreaBuilder.Property(t => t.CreatedAt)
                .IsRequired();

            commonAreaBuilder.Property(t => t.UpdateAt)
                .IsRequired(false);

            commonAreaBuilder.Property(c => c.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(150);

            commonAreaBuilder.Property(c => c.Description)
                .HasColumnName("Description")
                .IsRequired()
                .HasMaxLength(500);
        });

        builder.OwnsMany(p => p.Towers, towerBuilder => 
        {
            towerBuilder.ToTable("Tower");
            towerBuilder.WithOwner().HasForeignKey("PhysicalStructureId");
            towerBuilder.Property(t => t.Id).ValueGeneratedNever();
            towerBuilder.HasKey(t => t.Id);

            towerBuilder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(10);

            towerBuilder.Property(t => t.CreatedAt)
                .IsRequired();

            towerBuilder.Property(t => t.UpdateAt)
                .IsRequired(false);

            towerBuilder.Property(t => t.Number)
                .HasColumnName("Number")
                .IsRequired()
                .HasMaxLength(20);

            towerBuilder.Property(t => t.Floors)
                .HasColumnName("Floors")
                .IsRequired();
        });

    }
}
