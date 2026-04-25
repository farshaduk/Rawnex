using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class CompanyDocument : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public DocumentType Type { get; set; }
    public string FileName { get; set; } = default!;
    public string FileUrl { get; set; } = default!;
    public string? MimeType { get; set; }
    public long FileSizeBytes { get; set; }
    public VerificationStatus VerificationStatus { get; set; }
    public string? VerificationNotes { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}
