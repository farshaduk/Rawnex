using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Subdomain).IsRequired().HasMaxLength(100);
        builder.Property(t => t.LogoUrl).HasMaxLength(500);
        builder.Property(t => t.ContactEmail).HasMaxLength(256);
        builder.Property(t => t.ContactPhone).HasMaxLength(50);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Plan).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.CreatedBy).HasMaxLength(256);
        builder.Property(t => t.LastModifiedBy).HasMaxLength(256);
        builder.Property(t => t.DeletedBy).HasMaxLength(256);

        builder.HasIndex(t => t.Subdomain).IsUnique();
        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.HasOne(t => t.Settings)
            .WithOne(s => s.Tenant)
            .HasForeignKey<TenantSettings>(s => s.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Companies)
            .WithOne(c => c.Tenant)
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
