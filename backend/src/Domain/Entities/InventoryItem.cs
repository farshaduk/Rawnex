using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class InventoryItem : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ProductVariantId { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public decimal? ReservedQuantity { get; set; }
    public decimal AvailableQuantity => Quantity - (ReservedQuantity ?? 0);

    // Navigation
    public Warehouse Warehouse { get; set; } = default!;
    public Product Product { get; set; } = default!;
    public ProductVariant? ProductVariant { get; set; }
}
