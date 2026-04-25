using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class AuctionBidConfiguration : IEntityTypeConfiguration<AuctionBid>
{
    public void Configure(EntityTypeBuilder<AuctionBid> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Amount).HasPrecision(18, 4);
        builder.Property(b => b.Notes).HasMaxLength(1000);

        builder.HasIndex(b => b.AuctionId);
        builder.HasIndex(b => b.BidderCompanyId);

        builder.HasOne(b => b.BidderCompany)
            .WithMany()
            .HasForeignKey(b => b.BidderCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.BidderUser)
            .WithMany()
            .HasForeignKey(b => b.BidderUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
