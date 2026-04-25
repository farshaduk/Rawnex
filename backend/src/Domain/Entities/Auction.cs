using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Auction : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public AuctionType Type { get; set; }
    public AuctionStatus Status { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public decimal StartingPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public decimal? CurrentHighestBid { get; set; }
    public Currency PriceCurrency { get; set; }
    public decimal? MinBidIncrement { get; set; }
    public DateTime ScheduledStartAt { get; set; }
    public DateTime ScheduledEndAt { get; set; }
    public DateTime? ActualStartAt { get; set; }
    public DateTime? ActualEndAt { get; set; }
    public Guid? WinnerCompanyId { get; set; }
    public decimal? WinningBidAmount { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public Company? WinnerCompany { get; set; }
    public ICollection<AuctionBid> Bids { get; set; } = new List<AuctionBid>();
}
