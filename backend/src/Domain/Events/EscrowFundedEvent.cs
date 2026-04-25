using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class EscrowFundedEvent : BaseDomainEvent
{
    public Guid EscrowAccountId { get; }
    public Guid PurchaseOrderId { get; }
    public decimal Amount { get; }

    public EscrowFundedEvent(Guid escrowAccountId, Guid purchaseOrderId, decimal amount)
    {
        EscrowAccountId = escrowAccountId;
        PurchaseOrderId = purchaseOrderId;
        Amount = amount;
    }
}
