using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class KybVerification : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public VerificationStatus Status { get; set; }
    public string? CompanyRegistrationDocUrl { get; set; }
    public string? TaxCertificateUrl { get; set; }
    public string? FinancialStatementUrl { get; set; }
    public string? BankStatementUrl { get; set; }
    public string? ProductionLicenseUrl { get; set; }
    public string? ExportLicenseUrl { get; set; }
    public string? FactoryPhotoUrl { get; set; }
    public string? RejectionReason { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? NotesJson { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}
