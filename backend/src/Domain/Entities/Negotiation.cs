using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Negotiation : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid? RfqId { get; set; }
    public Guid? RfqResponseId { get; set; }
    public Guid? ListingId { get; set; }
    public NegotiationStatus Status { get; set; }
    public string? Subject { get; set; }

    // Final agreed terms
    public decimal? AgreedPrice { get; set; }
    public Currency? AgreedCurrency { get; set; }
    public decimal? AgreedQuantity { get; set; }
    public Incoterm? AgreedIncoterm { get; set; }
    public DateTime? AgreedAt { get; set; }

    // Navigation
    public Company BuyerCompany { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
    public Rfq? Rfq { get; set; }
    public RfqResponse? RfqResponse { get; set; }
    public Listing? Listing { get; set; }
    public ICollection<NegotiationMessage> Messages { get; set; } = new List<NegotiationMessage>();
}
