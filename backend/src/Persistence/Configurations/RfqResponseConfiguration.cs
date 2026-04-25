using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class RfqResponseConfiguration : IEntityTypeConfiguration<RfqResponse>
{
    public void Configure(EntityTypeBuilder<RfqResponse> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(r => r.ProposedPrice).HasPrecision(18, 4);
        builder.Property(r => r.PriceCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(r => r.PriceUnit).HasMaxLength(50);
        builder.Property(r => r.ProposedQuantity).HasPrecision(18, 4);
        builder.Property(r => r.UnitOfMeasure).HasMaxLength(20);
        builder.Property(r => r.Incoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(r => r.PaymentTerms).HasMaxLength(500);
        builder.Property(r => r.TechnicalSpecsJson).HasMaxLength(4000);
        builder.Property(r => r.Notes).HasMaxLength(2000);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(r => r.RfqId);
        builder.HasIndex(r => r.SellerCompanyId);
        builder.HasIndex(r => r.TenantId);

        builder.HasOne(r => r.SellerCompany)
            .WithMany()
            .HasForeignKey(r => r.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
