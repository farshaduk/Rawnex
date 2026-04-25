using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class BidPlacedEvent : BaseDomainEvent
{
    public Guid AuctionId { get; }
    public Guid BidderCompanyId { get; }
    public decimal Amount { get; }

    public BidPlacedEvent(Guid auctionId, Guid bidderCompanyId, decimal amount)
    {
        AuctionId = auctionId;
        BidderCompanyId = bidderCompanyId;
        Amount = amount;
    }
}
