using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.People;

namespace Infraestructure.Entity;

public class OwnerConfig : IEntityTypeConfiguration<OwnerAgg>
{
    public void Configure(EntityTypeBuilder<OwnerAgg> builder)
    {
        builder.ToTable("Owner");
        builder.HasKey(p => p.Id);

        // Campos heredados de Entity
        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdateAt)
            .IsRequired(false);

        // Campos propios del agregado
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.DocumentType)
            .IsRequired();

        builder.Property(p => p.DocumentNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(p => p.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.IdTermsAndCondition)
            .IsRequired(false);

        builder.Property(p => p.ResponseTermsAndCondition)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.MediaId)
            .IsRequired()
            .HasMaxLength(100);
    }
}
