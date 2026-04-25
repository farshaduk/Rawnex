using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class AuctionBid : BaseEntity
{
    public Guid AuctionId { get; set; }
    public Guid BidderCompanyId { get; set; }
    public Guid BidderUserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidAt { get; set; } = DateTime.UtcNow;
    public bool IsWinningBid { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Auction Auction { get; set; } = default!;
    public Company BidderCompany { get; set; } = default!;
    public ApplicationUser BidderUser { get; set; } = default!;
}
