using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class PurchaseOrder : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string OrderNumber { get; set; } = default!;
    public Guid BuyerCompanyId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid? NegotiationId { get; set; }
    public Guid? RfqId { get; set; }
    public Guid? ContractId { get; set; }
    public OrderStatus Status { get; set; }

    // Terms
    public Incoterm Incoterm { get; set; }
    public string? DeliveryLocation { get; set; }
    public DateTime? RequestedDeliveryDate { get; set; }
    public string? PaymentTerms { get; set; }
    public string? SpecialInstructions { get; set; }

    // Financials
    public decimal SubTotal { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? ShippingCost { get; set; }
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }

    // Confirmation
    public DateTime? ConfirmedAt { get; set; }
    public string? ConfirmedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation
    public Company BuyerCompany { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
    public Negotiation? Negotiation { get; set; }
    public Rfq? Rfq { get; set; }
    public Contract? Contract { get; set; }
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<OrderApproval> Approvals { get; set; } = new List<OrderApproval>();
}
