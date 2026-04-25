using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class RfqResponse : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid RfqId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public BidStatus Status { get; set; }
    public decimal ProposedPrice { get; set; }
    public Currency PriceCurrency { get; set; }
    public string? PriceUnit { get; set; }
    public decimal? ProposedQuantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public Incoterm? Incoterm { get; set; }
    public int? LeadTimeDays { get; set; }
    public string? PaymentTerms { get; set; }
    public string? TechnicalSpecsJson { get; set; }
    public string? Notes { get; set; }
    public DateTime? ValidUntil { get; set; }

    // Navigation
    public Rfq Rfq { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
}
