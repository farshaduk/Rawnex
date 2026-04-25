using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class KycVerification : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public VerificationStatus Status { get; set; }
    public string? FullName { get; set; }
    public string? NationalId { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? IdDocumentUrl { get; set; }
    public string? SelfieUrl { get; set; }
    public string? ProofOfAddressUrl { get; set; }
    public string? RejectionReason { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? NotesJson { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = default!;
}
