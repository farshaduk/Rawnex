using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(i => i.SubTotal).HasPrecision(18, 4);
        builder.Property(i => i.TaxAmount).HasPrecision(18, 4);
        builder.Property(i => i.DiscountAmount).HasPrecision(18, 4);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 4);
        builder.Property(i => i.PaidAmount).HasPrecision(18, 4);
        builder.Property(i => i.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(i => i.DocumentUrl).HasMaxLength(1000);
        builder.Property(i => i.Notes).HasMaxLength(2000);
        builder.Property(i => i.CreatedBy).HasMaxLength(256);
        builder.Property(i => i.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => i.PurchaseOrderId);
        builder.HasIndex(i => i.Status);

        builder.Ignore(i => i.DomainEvents);

        builder.HasOne(i => i.PurchaseOrder)
            .WithMany()
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.IssuerCompany)
            .WithMany()
            .HasForeignKey(i => i.IssuerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.RecipientCompany)
            .WithMany()
            .HasForeignKey(i => i.RecipientCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(item => item.Invoice)
            .HasForeignKey(item => item.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
