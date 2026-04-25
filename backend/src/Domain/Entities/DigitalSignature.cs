using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class DigitalSignature : BaseEntity
{
    public Guid ContractId { get; set; }
    public Guid SignerUserId { get; set; }
    public Guid SignerCompanyId { get; set; }
    public string SignerName { get; set; } = default!;
    public string SignerRole { get; set; } = default!;
    public string SignatureHash { get; set; } = default!;
    public string? IpAddress { get; set; }
    public DateTime SignedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Contract Contract { get; set; } = default!;
    public ApplicationUser SignerUser { get; set; } = default!;
    public Company SignerCompany { get; set; } = default!;
}
