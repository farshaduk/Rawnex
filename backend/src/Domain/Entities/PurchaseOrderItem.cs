using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public string ProductName { get; set; } = default!;
    public string? Sku { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public Currency Currency { get; set; }
    public decimal TotalPrice { get; set; }
    public string? SpecificationsJson { get; set; }

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public ProductVariant? ProductVariant { get; set; }
}
