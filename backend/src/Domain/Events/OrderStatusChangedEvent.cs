using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Events;

public class OrderStatusChangedEvent : BaseDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }

    public OrderStatusChangedEvent(Guid purchaseOrderId, OrderStatus oldStatus, OrderStatus newStatus)
    {
        PurchaseOrderId = purchaseOrderId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
