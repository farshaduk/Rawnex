using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class FraudScore : BaseAuditableEntity
{
    public Guid? UserId { get; set; }
    public Guid? CompanyId { get; set; }
    public FraudCheckType CheckType { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public decimal Score { get; set; }
    public string? DetailsJson { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceFingerprint { get; set; }
    public bool IsFlagged { get; set; }
    public string? FlagReason { get; set; }
    public bool IsReviewed { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navigation
    public ApplicationUser? User { get; set; }
    public Company? Company { get; set; }
}
