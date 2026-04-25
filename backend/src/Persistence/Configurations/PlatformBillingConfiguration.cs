using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class PlatformBillingConfiguration : IEntityTypeConfiguration<PlatformBilling>
{
    public void Configure(EntityTypeBuilder<PlatformBilling> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BillingReference).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Amount).HasPrecision(18, 4);
        builder.Property(b => b.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(b => b.CommissionType).HasConversion<string>().HasMaxLength(20);
        builder.Property(b => b.CommissionRate).HasPrecision(18, 4);
        builder.Property(b => b.InvoiceUrl).HasMaxLength(1000);
        builder.Property(b => b.CreatedBy).HasMaxLength(256);
        builder.Property(b => b.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => b.BillingReference).IsUnique();

        builder.HasOne(b => b.Tenant)
            .WithMany()
            .HasForeignKey(b => b.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.PurchaseOrder)
            .WithMany()
            .HasForeignKey(b => b.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
