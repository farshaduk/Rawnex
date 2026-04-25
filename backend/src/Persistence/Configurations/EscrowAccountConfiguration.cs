using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class EscrowAccountConfiguration : IEntityTypeConfiguration<EscrowAccount>
{
    public void Configure(EntityTypeBuilder<EscrowAccount> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.TotalAmount).HasPrecision(18, 4);
        builder.Property(e => e.FundedAmount).HasPrecision(18, 4);
        builder.Property(e => e.ReleasedAmount).HasPrecision(18, 4);
        builder.Property(e => e.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(e => e.TenantId);
        builder.HasIndex(e => e.PurchaseOrderId);

        builder.Ignore(e => e.DomainEvents);

        builder.HasOne(e => e.PurchaseOrder)
            .WithMany()
            .HasForeignKey(e => e.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.BuyerCompany)
            .WithMany()
            .HasForeignKey(e => e.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SellerCompany)
            .WithMany()
            .HasForeignKey(e => e.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Milestones)
            .WithOne(m => m.EscrowAccount)
            .HasForeignKey(m => m.EscrowAccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Payments)
            .WithOne(p => p.EscrowAccount)
            .HasForeignKey(p => p.EscrowAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
