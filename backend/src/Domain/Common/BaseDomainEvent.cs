using MediatR;

namespace Rawnex.Domain.Common;

public abstract class BaseDomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
