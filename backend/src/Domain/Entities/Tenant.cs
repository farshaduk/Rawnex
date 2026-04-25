using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Tenant : BaseAuditableEntity, IAggregateRoot, ISoftDeletable
{
    public string Name { get; set; } = default!;
    public string Subdomain { get; set; } = default!;
    public string? LogoUrl { get; set; }
    public TenantStatus Status { get; set; }
    public TenantPlan Plan { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public TenantSettings? Settings { get; set; }
    public ICollection<Company> Companies { get; set; } = new List<Company>();
}
