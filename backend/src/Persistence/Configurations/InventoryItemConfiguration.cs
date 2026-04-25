using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity).HasPrecision(18, 4);
        builder.Property(i => i.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(i => i.ReservedQuantity).HasPrecision(18, 4);
        builder.Property(i => i.CreatedBy).HasMaxLength(256);
        builder.Property(i => i.LastModifiedBy).HasMaxLength(256);

        builder.Ignore(i => i.AvailableQuantity);

        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => new { i.WarehouseId, i.ProductId });

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.ProductVariant)
            .WithMany()
            .HasForeignKey(i => i.ProductVariantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
