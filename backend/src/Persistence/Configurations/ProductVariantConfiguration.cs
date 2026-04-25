using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name).IsRequired().HasMaxLength(200);
        builder.Property(v => v.Sku).HasMaxLength(50);
        builder.Property(v => v.Origin).HasMaxLength(100);
        builder.Property(v => v.PurityGrade).HasMaxLength(50);
        builder.Property(v => v.Packaging).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.Price).HasPrecision(18, 4);
        builder.Property(v => v.PriceCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(v => v.AvailableQuantity).HasPrecision(18, 4);
        builder.Property(v => v.UnitOfMeasure).HasMaxLength(20);
        builder.Property(v => v.CreatedBy).HasMaxLength(256);
        builder.Property(v => v.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(v => v.ProductId);
    }
}
