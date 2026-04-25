using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class ProductCertification : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public string CertificationType { get; set; } = default!;
    public string? CertificationBody { get; set; }
    public string? CertificateNumber { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? IssuedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsVerified { get; set; }

    // Navigation
    public Product Product { get; set; } = default!;
}
