using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName).IsRequired().HasMaxLength(300);
        builder.Property(i => i.Sku).HasMaxLength(50);
        builder.Property(i => i.Quantity).HasPrecision(18, 4);
        builder.Property(i => i.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 4);
        builder.Property(i => i.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(i => i.TotalPrice).HasPrecision(18, 4);
        builder.Property(i => i.SpecificationsJson).HasMaxLength(4000);

        builder.HasIndex(i => i.PurchaseOrderId);

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
