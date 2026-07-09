using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.BoundedContext.Tenancy;

namespace Infraestructure.Entity;

public class ManagementCompanyConfig : IEntityTypeConfiguration<ManagementCompanyAgg>
{
    public void Configure(EntityTypeBuilder<ManagementCompanyAgg> builder)
    {
        builder.ToTable("ManagementCompanies");
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

        builder.HasIndex(p => p.Nit)
            .IsUnique();

        builder.Property(p => p.ContactEmail)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.ContactPhone)
            .IsRequired()
            .HasMaxLength(30);
    }
}
