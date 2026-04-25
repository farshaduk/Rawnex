using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class CompanyMember : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? JobTitle { get; set; }
    public bool IsCompanyAdmin { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Company Company { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public Department? Department { get; set; }
}
