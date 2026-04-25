using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class TrustedPartner : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid PartnerCompanyId { get; set; }
    public string? Notes { get; set; }
    public bool IsApproved { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
    public Company PartnerCompany { get; set; } = default!;
}
