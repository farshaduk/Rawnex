using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class UboRecord : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public string FullName { get; set; } = default!;
    public string? NationalId { get; set; }
    public string? PassportNumber { get; set; }
    public string? Nationality { get; set; }
    public decimal OwnershipPercentage { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? VerifiedAt { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
}
