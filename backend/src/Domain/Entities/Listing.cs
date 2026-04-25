using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Listing : BaseAuditableEntity, IAggregateRoot, ITenantEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid ProductId { get; set; }
    public ListingType Type { get; set; }
    public ListingStatus Status { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public decimal Price { get; set; }
    public Currency PriceCurrency { get; set; }
    public string? PriceUnit { get; set; }
    public decimal? MinOrderQuantity { get; set; }
    public Incoterm Incoterm { get; set; }
    public string? DeliveryLocation { get; set; }
    public int? LeadTimeDays { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Forward contract specific
    public DateTime? DeliveryStartDate { get; set; }
    public DateTime? DeliveryEndDate { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
