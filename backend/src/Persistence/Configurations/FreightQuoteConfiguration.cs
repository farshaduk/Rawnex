using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class FreightQuoteConfiguration : IEntityTypeConfiguration<FreightQuote>
{
    public void Configure(EntityTypeBuilder<FreightQuote> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.CarrierName).IsRequired().HasMaxLength(200);
        builder.Property(f => f.TransportMode).HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.OriginCity).HasMaxLength(100);
        builder.Property(f => f.OriginCountry).HasMaxLength(100);
        builder.Property(f => f.DestinationCity).HasMaxLength(100);
        builder.Property(f => f.DestinationCountry).HasMaxLength(100);
        builder.Property(f => f.QuotedPrice).HasPrecision(18, 4);
        builder.Property(f => f.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(f => f.CreatedBy).HasMaxLength(256);
        builder.Property(f => f.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(f => f.TenantId);
        builder.HasIndex(f => f.PurchaseOrderId);

        builder.HasOne(f => f.PurchaseOrder)
            .WithMany()
            .HasForeignKey(f => f.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
