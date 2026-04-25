using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Dispute : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid FiledByCompanyId { get; set; }
    public Guid FiledByUserId { get; set; }
    public Guid AgainstCompanyId { get; set; }
    public string DisputeNumber { get; set; } = default!;
    public DisputeStatus Status { get; set; }
    public DisputeReason Reason { get; set; }
    public string Description { get; set; } = default!;
    public decimal? ClaimedAmount { get; set; }
    public Currency? ClaimedCurrency { get; set; }

    // Resolution
    public DisputeResolution? Resolution { get; set; }
    public string? ResolutionNotes { get; set; }
    public decimal? ResolvedAmount { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? ResolvedBy { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Company FiledByCompany { get; set; } = default!;
    public ApplicationUser FiledByUser { get; set; } = default!;
    public Company AgainstCompany { get; set; } = default!;
    public ICollection<DisputeEvidence> Evidence { get; set; } = new List<DisputeEvidence>();
}
