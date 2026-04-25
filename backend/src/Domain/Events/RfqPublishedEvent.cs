using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class RfqPublishedEvent : BaseDomainEvent
{
    public Guid RfqId { get; }
    public Guid BuyerCompanyId { get; }

    public RfqPublishedEvent(Guid rfqId, Guid buyerCompanyId)
    {
        RfqId = rfqId;
        BuyerCompanyId = buyerCompanyId;
    }
}
