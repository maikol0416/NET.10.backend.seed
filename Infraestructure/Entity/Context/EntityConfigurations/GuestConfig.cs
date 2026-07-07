using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.People;

namespace Infraestructure.Entity;

public class GuestConfig : IEntityTypeConfiguration<GuestAgg>
{
    public void Configure(EntityTypeBuilder<GuestAgg> builder)
    {
        builder.ToTable("Guest");
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

        builder.Property(p => p.TermsAndCondition)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.ResponseTermsAndCondition)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.MediaId)
            .IsRequired()
            .HasMaxLength(100);

        // OwnsMany: Entidad hija GuestPermissionEntity
        builder.OwnsMany(p => p.GuestPermissions, guestPermissionBuilder =>
        {
            guestPermissionBuilder.ToTable("GuestPermission");
            guestPermissionBuilder.WithOwner().HasForeignKey("GuestId");
            guestPermissionBuilder.Property(t => t.Id).ValueGeneratedNever();
            guestPermissionBuilder.HasKey(t => t.Id);

            guestPermissionBuilder.Property(t => t.Status)
                .IsRequired()
                .HasMaxLength(10);

            guestPermissionBuilder.Property(t => t.CreatedAt)
                .IsRequired();

            guestPermissionBuilder.Property(t => t.UpdateAt)
                .IsRequired(false);

            guestPermissionBuilder.Property(t => t.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            guestPermissionBuilder.Property(t => t.EndDate)
                .HasColumnName("EndDate")
                .IsRequired();
        });
    }
}
