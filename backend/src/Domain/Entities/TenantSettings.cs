using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class TenantSettings : BaseEntity
{
    public Guid TenantId { get; set; }
    public string DefaultCurrency { get; set; } = "USD";
    public string DefaultLanguage { get; set; } = "fa";
    public string TimeZone { get; set; } = "Asia/Tehran";
    public bool RequireMfa { get; set; }
    public bool RequireKyc { get; set; } = true;
    public int MaxUsersAllowed { get; set; } = 50;
    public string? CustomBrandingJson { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
}
