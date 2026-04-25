using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class Rating : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ReviewerCompanyId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public Guid ReviewedCompanyId { get; set; }

    // Scores (1-5)
    public int OverallScore { get; set; }
    public int? QualityScore { get; set; }
    public int? DeliveryScore { get; set; }
    public int? CommunicationScore { get; set; }
    public int? ValueScore { get; set; }

    public string? Comment { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime? ResponseAt { get; set; }
    public string? ResponseComment { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Company ReviewerCompany { get; set; } = default!;
    public ApplicationUser ReviewerUser { get; set; } = default!;
    public Company ReviewedCompany { get; set; } = default!;
}
