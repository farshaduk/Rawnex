using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class FreightQuote : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string CarrierName { get; set; } = default!;
    public TransportMode TransportMode { get; set; }
    public string? OriginCity { get; set; }
    public string? OriginCountry { get; set; }
    public string? DestinationCity { get; set; }
    public string? DestinationCountry { get; set; }
    public decimal QuotedPrice { get; set; }
    public Currency Currency { get; set; }
    public int? EstimatedTransitDays { get; set; }
    public DateTime? ValidUntil { get; set; }
    public bool IsSelected { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
}
