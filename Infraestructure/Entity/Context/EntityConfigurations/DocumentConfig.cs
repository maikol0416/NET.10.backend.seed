using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.DocumentManagement;

namespace Infraestructure.Entity;

public class DocumentConfig : IEntityTypeConfiguration<DocumentAgg>
{
    public void Configure(EntityTypeBuilder<DocumentAgg> builder)
    {
        builder.ToTable("Document");
        builder.HasKey(p => p.Id);

        // ✅ Campos heredados de Entity
        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(1);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdateAt)
            .IsRequired(false);

        // Campos propios del agregado
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(p => p.Path)
            .IsRequired()
            .HasMaxLength(500);

        // OwnsMany: SignatureValueObject (1 a muchos)
        builder.OwnsMany(p => p.Signatures, sigBuilder =>
        {
            sigBuilder.ToTable("DocumentSignature");
            sigBuilder.WithOwner().HasForeignKey("DocumentId");
            sigBuilder.Property<int>("Id");
            sigBuilder.HasKey("Id");

            sigBuilder.Property(s => s.Name)
                .HasColumnName("Name")
                .IsRequired()
                .HasMaxLength(200);

            sigBuilder.Property(s => s.Rol)
                .HasColumnName("Rol")
                .IsRequired()
                .HasMaxLength(100);
        });
    }
}
