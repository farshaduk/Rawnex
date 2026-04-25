using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class RfqConfiguration : IEntityTypeConfiguration<Rfq>
{
    public void Configure(EntityTypeBuilder<Rfq> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RfqNumber).IsRequired().HasMaxLength(50);
        builder.Property(r => r.Title).IsRequired().HasMaxLength(300);
        builder.Property(r => r.Description).HasMaxLength(4000);
        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.Visibility).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.MaterialName).HasMaxLength(300);
        builder.Property(r => r.RequiredSpecsJson).HasMaxLength(4000);
        builder.Property(r => r.RequiredQuantity).HasPrecision(18, 4);
        builder.Property(r => r.UnitOfMeasure).HasMaxLength(20);
        builder.Property(r => r.BudgetMin).HasPrecision(18, 4);
        builder.Property(r => r.BudgetMax).HasPrecision(18, 4);
        builder.Property(r => r.BudgetCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(r => r.PreferredIncoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(r => r.DeliveryLocation).HasMaxLength(500);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => r.BuyerCompanyId);
        builder.HasIndex(r => r.RfqNumber).IsUnique();
        builder.HasIndex(r => r.Status);

        builder.Ignore(r => r.DomainEvents);

        builder.HasOne(r => r.BuyerCompany)
            .WithMany()
            .HasForeignKey(r => r.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.AwardedToCompany)
            .WithMany()
            .HasForeignKey(r => r.AwardedToCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Responses)
            .WithOne(resp => resp.Rfq)
            .HasForeignKey(resp => resp.RfqId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.Invitations)
            .WithOne(inv => inv.Rfq)
            .HasForeignKey(inv => inv.RfqId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
