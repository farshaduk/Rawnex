using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class EscrowMilestoneCompletedEvent : BaseDomainEvent
{
    public Guid EscrowAccountId { get; }
    public Guid MilestoneId { get; }
    public decimal ReleasedAmount { get; }

    public EscrowMilestoneCompletedEvent(Guid escrowAccountId, Guid milestoneId, decimal releasedAmount)
    {
        EscrowAccountId = escrowAccountId;
        MilestoneId = milestoneId;
        ReleasedAmount = releasedAmount;
    }
}
