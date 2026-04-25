using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
{
    public void Configure(EntityTypeBuilder<Auction> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title).IsRequired().HasMaxLength(300);
        builder.Property(a => a.Description).HasMaxLength(4000);
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(a => a.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Quantity).HasPrecision(18, 4);
        builder.Property(a => a.StartingPrice).HasPrecision(18, 4);
        builder.Property(a => a.ReservePrice).HasPrecision(18, 4);
        builder.Property(a => a.CurrentHighestBid).HasPrecision(18, 4);
        builder.Property(a => a.PriceCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.MinBidIncrement).HasPrecision(18, 4);
        builder.Property(a => a.WinningBidAmount).HasPrecision(18, 4);
        builder.Property(a => a.CreatedBy).HasMaxLength(256);
        builder.Property(a => a.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => a.CompanyId);
        builder.HasIndex(a => a.Status);

        builder.Ignore(a => a.DomainEvents);

        builder.HasOne(a => a.Company)
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.WinnerCompany)
            .WithMany()
            .HasForeignKey(a => a.WinnerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.Bids)
            .WithOne(b => b.Auction)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
