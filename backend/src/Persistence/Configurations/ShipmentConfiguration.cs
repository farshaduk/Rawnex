using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShipmentNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(s => s.TransportMode).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.Incoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(s => s.CarrierName).HasMaxLength(200);
        builder.Property(s => s.CarrierTrackingNumber).HasMaxLength(100);
        builder.Property(s => s.BillOfLadingNumber).HasMaxLength(100);
        builder.Property(s => s.BillOfLadingUrl).HasMaxLength(1000);
        builder.Property(s => s.ContainerNumber).HasMaxLength(50);
        builder.Property(s => s.SealNumber).HasMaxLength(50);
        builder.Property(s => s.ContainerSealPhotoUrl).HasMaxLength(1000);
        builder.Property(s => s.OriginAddress).HasMaxLength(500);
        builder.Property(s => s.OriginCity).HasMaxLength(100);
        builder.Property(s => s.OriginCountry).HasMaxLength(100);
        builder.Property(s => s.DestinationAddress).HasMaxLength(500);
        builder.Property(s => s.DestinationCity).HasMaxLength(100);
        builder.Property(s => s.DestinationCountry).HasMaxLength(100);
        builder.Property(s => s.GrossWeightKg).HasPrecision(18, 4);
        builder.Property(s => s.NetWeightKg).HasPrecision(18, 4);
        builder.Property(s => s.ShippingCost).HasPrecision(18, 4);
        builder.Property(s => s.InsuranceCost).HasPrecision(18, 4);
        builder.Property(s => s.CostCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(s => s.CarbonFootprintKg).HasPrecision(18, 4);
        builder.Property(s => s.CreatedBy).HasMaxLength(256);
        builder.Property(s => s.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.ShipmentNumber).IsUnique();
        builder.HasIndex(s => s.PurchaseOrderId);
        builder.HasIndex(s => s.Status);

        builder.Ignore(s => s.DomainEvents);

        builder.HasOne(s => s.PurchaseOrder)
            .WithMany()
            .HasForeignKey(s => s.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SellerCompany)
            .WithMany()
            .HasForeignKey(s => s.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.BuyerCompany)
            .WithMany()
            .HasForeignKey(s => s.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.TrackingEvents)
            .WithOne(t => t.Shipment)
            .HasForeignKey(t => t.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Batches)
            .WithOne(b => b.Shipment)
            .HasForeignKey(b => b.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
