using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ContractNumber).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Title).IsRequired().HasMaxLength(300);
        builder.Property(c => c.Description).HasMaxLength(4000);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.TotalValue).HasPrecision(18, 4);
        builder.Property(c => c.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(c => c.Incoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(c => c.PaymentTerms).HasMaxLength(500);
        builder.Property(c => c.DeliveryTerms).HasMaxLength(500);
        builder.Property(c => c.QualityTerms).HasMaxLength(500);
        builder.Property(c => c.TerminationReason).HasMaxLength(1000);
        builder.Property(c => c.DocumentUrl).HasMaxLength(1000);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.ContractNumber).IsUnique();
        builder.HasIndex(c => c.BuyerCompanyId);
        builder.HasIndex(c => c.SellerCompanyId);
        builder.HasIndex(c => c.Status);

        builder.Ignore(c => c.DomainEvents);

        builder.HasOne(c => c.BuyerCompany)
            .WithMany()
            .HasForeignKey(c => c.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SellerCompany)
            .WithMany()
            .HasForeignKey(c => c.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.PurchaseOrder)
            .WithMany()
            .HasForeignKey(c => c.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Clauses)
            .WithOne(cl => cl.Contract)
            .HasForeignKey(cl => cl.ContractId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Signatures)
            .WithOne(s => s.Contract)
            .HasForeignKey(s => s.ContractId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
