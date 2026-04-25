using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Description).IsRequired().HasMaxLength(500);
        builder.Property(i => i.Quantity).HasPrecision(18, 4);
        builder.Property(i => i.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(i => i.UnitPrice).HasPrecision(18, 4);
        builder.Property(i => i.TotalPrice).HasPrecision(18, 4);
        builder.Property(i => i.Currency).HasConversion<string>().HasMaxLength(10);

        builder.HasIndex(i => i.InvoiceId);
    }
}
