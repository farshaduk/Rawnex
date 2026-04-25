using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Rfq : BaseAuditableEntity, IAggregateRoot, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public string RfqNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public RfqStatus Status { get; set; }
    public RfqVisibility Visibility { get; set; }

    // Material requirements
    public Guid? CategoryId { get; set; }
    public string? MaterialName { get; set; }
    public string? RequiredSpecsJson { get; set; }
    public decimal? RequiredQuantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? BudgetMin { get; set; }
    public decimal? BudgetMax { get; set; }
    public Currency BudgetCurrency { get; set; }

    // Delivery
    public Incoterm? PreferredIncoterm { get; set; }
    public string? DeliveryLocation { get; set; }
    public DateTime? DeliveryDeadline { get; set; }

    // Timeline
    public DateTime? ResponseDeadline { get; set; }
    public DateTime? AwardedAt { get; set; }
    public Guid? AwardedToCompanyId { get; set; }

    // Navigation
    public Company BuyerCompany { get; set; } = default!;
    public ProductCategory? Category { get; set; }
    public Company? AwardedToCompany { get; set; }
    public ICollection<RfqResponse> Responses { get; set; } = new List<RfqResponse>();
    public ICollection<RfqInvitation> Invitations { get; set; } = new List<RfqInvitation>();
}
