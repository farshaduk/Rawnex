using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(o => o.Incoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(o => o.DeliveryLocation).HasMaxLength(500);
        builder.Property(o => o.PaymentTerms).HasMaxLength(500);
        builder.Property(o => o.SpecialInstructions).HasMaxLength(2000);
        builder.Property(o => o.SubTotal).HasPrecision(18, 4);
        builder.Property(o => o.TaxAmount).HasPrecision(18, 4);
        builder.Property(o => o.ShippingCost).HasPrecision(18, 4);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 4);
        builder.Property(o => o.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(o => o.ConfirmedBy).HasMaxLength(256);
        builder.Property(o => o.CancellationReason).HasMaxLength(1000);
        builder.Property(o => o.CreatedBy).HasMaxLength(256);
        builder.Property(o => o.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(o => o.TenantId);
        builder.HasIndex(o => o.OrderNumber).IsUnique();
        builder.HasIndex(o => o.BuyerCompanyId);
        builder.HasIndex(o => o.SellerCompanyId);
        builder.HasIndex(o => o.Status);

        builder.Ignore(o => o.DomainEvents);

        builder.HasOne(o => o.BuyerCompany)
            .WithMany()
            .HasForeignKey(o => o.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.SellerCompany)
            .WithMany()
            .HasForeignKey(o => o.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Negotiation)
            .WithMany()
            .HasForeignKey(o => o.NegotiationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Rfq)
            .WithMany()
            .HasForeignKey(o => o.RfqId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Contract)
            .WithMany()
            .HasForeignKey(o => o.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.PurchaseOrder)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Approvals)
            .WithOne(a => a.PurchaseOrder)
            .HasForeignKey(a => a.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
