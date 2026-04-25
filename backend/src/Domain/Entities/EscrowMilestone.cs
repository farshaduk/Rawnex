using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class EscrowMilestone : BaseAuditableEntity
{
    public Guid EscrowAccountId { get; set; }
    public MilestoneType Type { get; set; }
    public MilestoneStatus Status { get; set; }
    public string Description { get; set; } = default!;
    public decimal ReleasePercentage { get; set; }
    public decimal ReleaseAmount { get; set; }
    public int SortOrder { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedBy { get; set; }
    public string? EvidenceUrl { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public EscrowAccount EscrowAccount { get; set; } = default!;
}
