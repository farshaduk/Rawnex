using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Events;

public class FraudDetectedEvent : BaseDomainEvent
{
    public Guid? UserId { get; }
    public Guid? CompanyId { get; }
    public RiskLevel RiskLevel { get; }
    public string Reason { get; }

    public FraudDetectedEvent(Guid? userId, Guid? companyId, RiskLevel riskLevel, string reason)
    {
        UserId = userId;
        CompanyId = companyId;
        RiskLevel = riskLevel;
        Reason = reason;
    }
}
