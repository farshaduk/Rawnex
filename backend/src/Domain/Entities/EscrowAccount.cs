using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class EscrowAccount : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public EscrowStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal FundedAmount { get; set; }
    public decimal ReleasedAmount { get; set; }
    public Currency Currency { get; set; }
    public DateTime? FundedAt { get; set; }
    public DateTime? FullyReleasedAt { get; set; }
    public DateTime? RefundedAt { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Company BuyerCompany { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
    public ICollection<EscrowMilestone> Milestones { get; set; } = new List<EscrowMilestone>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
