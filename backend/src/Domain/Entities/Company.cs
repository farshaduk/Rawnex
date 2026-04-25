using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Company : BaseAuditableEntity, IAggregateRoot, ITenantEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public string LegalName { get; set; } = default!;
    public string? TradeName { get; set; }
    public string RegistrationNumber { get; set; } = default!;
    public string? TaxId { get; set; }
    public CompanyType Type { get; set; }
    public CompanyStatus Status { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Description { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    // Contact
    public string? Phone { get; set; }
    public string? Email { get; set; }

    // Financial
    public decimal? CreditLimit { get; set; }
    public Currency DefaultCurrency { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankIban { get; set; }
    public string? BankSwiftCode { get; set; }

    // Parent company (for subsidiaries)
    public Guid? ParentCompanyId { get; set; }

    // Verification
    public VerificationStatus VerificationStatus { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public string? VerifiedBy { get; set; }

    // Score
    public decimal? EsgScore { get; set; }
    public decimal? TrustScore { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public Company? ParentCompany { get; set; }
    public ICollection<Company> Subsidiaries { get; set; } = new List<Company>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<CompanyDocument> Documents { get; set; } = new List<CompanyDocument>();
    public ICollection<UboRecord> UboRecords { get; set; } = new List<UboRecord>();
    public ICollection<CompanyMember> Members { get; set; } = new List<CompanyMember>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<TrustedPartner> TrustedPartners { get; set; } = new List<TrustedPartner>();
}
