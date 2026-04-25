using Rawnex.Domain.Common;

namespace Rawnex.Domain.Events;

public sealed class TokenReuseDetectedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Guid TokenFamily { get; }
    public string? IpAddress { get; }

    public TokenReuseDetectedEvent(Guid userId, Guid tokenFamily, string? ipAddress)
    {
        UserId = userId;
        TokenFamily = tokenFamily;
        IpAddress = ipAddress;
    }
}
