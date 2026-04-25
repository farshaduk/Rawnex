using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class QualityReport : BaseAuditableEntity
{
    public Guid QualityInspectionId { get; set; }
    public string Title { get; set; } = default!;
    public string? Summary { get; set; }
    public string? DetailedFindingsJson { get; set; }
    public string? FileUrl { get; set; }
    public bool PassedOverallCheck { get; set; }

    // Navigation
    public QualityInspection QualityInspection { get; set; } = default!;
    public ICollection<LabTestResult> LabTestResults { get; set; } = new List<LabTestResult>();
}
