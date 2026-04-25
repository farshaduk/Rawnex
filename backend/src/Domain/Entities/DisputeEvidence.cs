using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class DisputeEvidence : BaseAuditableEntity
{
    public Guid DisputeId { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public string FileUrl { get; set; } = default!;
    public string? MimeType { get; set; }
    public long FileSizeBytes { get; set; }

    // Navigation
    public Dispute Dispute { get; set; } = default!;
    public ApplicationUser UploadedByUser { get; set; } = default!;
}
