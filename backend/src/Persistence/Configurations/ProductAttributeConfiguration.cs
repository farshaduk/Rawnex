using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
{
    public void Configure(EntityTypeBuilder<ProductAttribute> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Key).IsRequired().HasMaxLength(100);
        builder.Property(a => a.Value).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Unit).HasMaxLength(50);

        builder.HasIndex(a => a.ProductId);
    }
}
