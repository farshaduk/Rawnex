using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public class CompanyVerifiedEvent : BaseDomainEvent
{
    public Guid CompanyId { get; }
    public Guid TenantId { get; }

    public CompanyVerifiedEvent(Guid companyId, Guid tenantId)
    {
        CompanyId = companyId;
        TenantId = tenantId;
    }
}
