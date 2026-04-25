using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class OrderApproval : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public int StepOrder { get; set; }
    public string StepName { get; set; } = default!;
    public Guid? ApproverUserId { get; set; }
    public ApprovalStatus Status { get; set; }
    public string? Comments { get; set; }
    public DateTime? DecidedAt { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public ApplicationUser? ApproverUser { get; set; }
}
