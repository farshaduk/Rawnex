using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class PriceAlert : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public decimal TargetPrice { get; set; }
    public Currency Currency { get; set; }
    public bool AlertWhenBelow { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public DateTime? LastTriggeredAt { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
