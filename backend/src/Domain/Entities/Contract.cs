using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Contract : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public string ContractNumber { get; set; } = default!;
    public Guid BuyerCompanyId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public ContractType Type { get; set; }
    public ContractStatus Status { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }

    // Terms
    public decimal TotalValue { get; set; }
    public Currency Currency { get; set; }
    public Incoterm? Incoterm { get; set; }
    public string? PaymentTerms { get; set; }
    public string? DeliveryTerms { get; set; }
    public string? QualityTerms { get; set; }

    // Timeline
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? SignedAt { get; set; }
    public DateTime? TerminatedAt { get; set; }
    public string? TerminationReason { get; set; }

    // Document
    public string? DocumentUrl { get; set; }
    public int Version { get; set; } = 1;

    // Navigation
    public Company BuyerCompany { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ICollection<ContractClause> Clauses { get; set; } = new List<ContractClause>();
    public ICollection<DigitalSignature> Signatures { get; set; } = new List<DigitalSignature>();
}
