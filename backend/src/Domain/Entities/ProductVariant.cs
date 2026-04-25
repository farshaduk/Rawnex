using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class ProductVariant : BaseAuditableEntity
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Sku { get; set; }
    public string? Origin { get; set; }
    public string? PurityGrade { get; set; }
    public PackagingType? Packaging { get; set; }
    public decimal? Price { get; set; }
    public Currency PriceCurrency { get; set; }
    public decimal? AvailableQuantity { get; set; }
    public string? UnitOfMeasure { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Product Product { get; set; } = default!;
}
