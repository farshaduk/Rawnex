using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class SavedSearch : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = default!;
    public string SearchCriteriaJson { get; set; } = default!;
    public bool AlertEnabled { get; set; }
    public DateTime? LastAlertSentAt { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = default!;
}
