using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class EsgScore : BaseAuditableEntity
{
    public Guid CompanyId { get; set; }
    public decimal EnvironmentalScore { get; set; }
    public decimal SocialScore { get; set; }
    public decimal GovernanceScore { get; set; }
    public decimal OverallScore { get; set; }
    public string? AssessmentDetailsJson { get; set; }
    public DateTime AssessmentDate { get; set; }
    public DateTime? NextAssessmentDue { get; set; }
    public string? CertificationUrl { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}
