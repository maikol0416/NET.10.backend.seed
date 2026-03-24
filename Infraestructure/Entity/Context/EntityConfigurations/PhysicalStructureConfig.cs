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
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.OwnsOne(p => p.Location, locationBuilder =>
        {
            locationBuilder.Property(l => l.Street)
                .HasColumnName("Street")
                .IsRequired()
                .HasMaxLength(200);

            locationBuilder.Property(l => l.Number)
                .HasColumnName("StreetNumber")
                .IsRequired()
                .HasMaxLength(20);
        });
    }
}
