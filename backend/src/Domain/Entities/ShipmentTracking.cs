using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class ShipmentTracking : BaseEntity
{
    public Guid ShipmentId { get; set; }
    public ShipmentStatus Status { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Source { get; set; }

    // Navigation
    public Shipment Shipment { get; set; } = default!;
}
