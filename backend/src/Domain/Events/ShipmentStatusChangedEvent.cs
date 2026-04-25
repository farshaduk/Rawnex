using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Events;

public class ShipmentStatusChangedEvent : BaseDomainEvent
{
    public Guid ShipmentId { get; }
    public ShipmentStatus OldStatus { get; }
    public ShipmentStatus NewStatus { get; }

    public ShipmentStatusChangedEvent(Guid shipmentId, ShipmentStatus oldStatus, ShipmentStatus newStatus)
    {
        ShipmentId = shipmentId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
