using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class NegotiationConfiguration : IEntityTypeConfiguration<Negotiation>
{
    public void Configure(EntityTypeBuilder<Negotiation> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Subject).HasMaxLength(300);
        builder.Property(n => n.AgreedPrice).HasPrecision(18, 4);
        builder.Property(n => n.AgreedCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(n => n.AgreedQuantity).HasPrecision(18, 4);
        builder.Property(n => n.AgreedIncoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(n => n.CreatedBy).HasMaxLength(256);
        builder.Property(n => n.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(n => n.TenantId);
        builder.HasIndex(n => n.BuyerCompanyId);
        builder.HasIndex(n => n.SellerCompanyId);
        builder.HasIndex(n => n.Status);

        builder.Ignore(n => n.DomainEvents);

        builder.HasOne(n => n.BuyerCompany)
            .WithMany()
            .HasForeignKey(n => n.BuyerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.SellerCompany)
            .WithMany()
            .HasForeignKey(n => n.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Rfq)
            .WithMany()
            .HasForeignKey(n => n.RfqId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.RfqResponse)
            .WithMany()
            .HasForeignKey(n => n.RfqResponseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Listing)
            .WithMany()
            .HasForeignKey(n => n.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(n => n.Messages)
            .WithOne(m => m.Negotiation)
            .HasForeignKey(m => m.NegotiationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
