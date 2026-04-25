using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public sealed class UserLoggedInEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public string IpAddress { get; }
    public string DeviceInfo { get; }

    public UserLoggedInEvent(Guid userId, string ipAddress, string deviceInfo)
    {
        UserId = userId;
        IpAddress = ipAddress;
        DeviceInfo = deviceInfo;
    }
}
