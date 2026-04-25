using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class LabTestResult : BaseAuditableEntity
{
    public Guid QualityReportId { get; set; }
    public string TestName { get; set; } = default!;
    public string? TestMethod { get; set; }
    public string? Parameter { get; set; }
    public string? ExpectedValue { get; set; }
    public string? ActualValue { get; set; }
    public string? Unit { get; set; }
    public bool Passed { get; set; }
    public string? LabName { get; set; }
    public string? CertificateUrl { get; set; }
    public DateTime? TestDate { get; set; }

    // Navigation
    public QualityReport QualityReport { get; set; } = default!;
}
