using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class DisputeFiledEvent : BaseDomainEvent
{
    public Guid DisputeId { get; }
    public Guid PurchaseOrderId { get; }
    public Guid FiledByCompanyId { get; }

    public DisputeFiledEvent(Guid disputeId, Guid purchaseOrderId, Guid filedByCompanyId)
    {
        DisputeId = disputeId;
        PurchaseOrderId = purchaseOrderId;
        FiledByCompanyId = filedByCompanyId;
    }
}
