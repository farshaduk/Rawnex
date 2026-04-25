using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class SanctionCheck : BaseAuditableEntity
{
    public Guid CompanyId { get; set; }
    public string CheckedAgainst { get; set; } = default!;
    public bool IsMatch { get; set; }
    public string? MatchDetailsJson { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public DateTime? NextCheckDue { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}
