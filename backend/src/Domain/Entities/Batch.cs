using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class Batch : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid ProductId { get; set; }
    public string BatchNumber { get; set; } = default!;
    public string? LotNumber { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = default!;
    public DateTime? ManufacturedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? CoaFileUrl { get; set; }
    public string? Origin { get; set; }
    public string? QualityGrade { get; set; }

    // Navigation
    public Shipment Shipment { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
