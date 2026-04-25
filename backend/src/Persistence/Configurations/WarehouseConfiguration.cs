using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Code).HasMaxLength(50);
        builder.Property(w => w.AddressLine1).HasMaxLength(500);
        builder.Property(w => w.City).HasMaxLength(100);
        builder.Property(w => w.Country).HasMaxLength(100);
        builder.Property(w => w.ContactPhone).HasMaxLength(20);
        builder.Property(w => w.CapacityTons).HasPrecision(18, 4);
        builder.Property(w => w.CreatedBy).HasMaxLength(256);
        builder.Property(w => w.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(w => w.TenantId);
        builder.HasIndex(w => w.CompanyId);

        builder.HasOne(w => w.Company)
            .WithMany()
            .HasForeignKey(w => w.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(w => w.InventoryItems)
            .WithOne(i => i.Warehouse)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
