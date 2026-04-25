using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PaymentReference).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Method).HasConversion<string>().HasMaxLength(30);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Amount).HasPrecision(18, 4);
        builder.Property(p => p.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.ExchangeRate).HasPrecision(18, 6);
        builder.Property(p => p.TransactionId).HasMaxLength(200);
        builder.Property(p => p.GatewayResponse).HasMaxLength(4000);
        builder.Property(p => p.FailureReason).HasMaxLength(1000);
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.PaymentReference).IsUnique();
        builder.HasIndex(p => p.Status);

        builder.Ignore(p => p.DomainEvents);

        builder.HasOne(p => p.PurchaseOrder)
            .WithMany()
            .HasForeignKey(p => p.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PayerCompany)
            .WithMany()
            .HasForeignKey(p => p.PayerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.PayeeCompany)
            .WithMany()
            .HasForeignKey(p => p.PayeeCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
