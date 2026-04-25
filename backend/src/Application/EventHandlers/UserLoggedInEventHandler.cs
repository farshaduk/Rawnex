using MediatR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class UserLoggedInEventHandler : INotificationHandler<UserLoggedInEvent>
{
    private readonly IAuditService _audit;
    private readonly IFraudScoringService _fraud;
    private readonly ILogger<UserLoggedInEventHandler> _logger;

    public UserLoggedInEventHandler(
        IAuditService audit,
        IFraudScoringService fraud,
        ILogger<UserLoggedInEventHandler> logger)
    {
        _audit = audit;
        _fraud = fraud;
        _logger = logger;
    }

    public async Task Handle(UserLoggedInEvent notification, CancellationToken ct)
    {
        await _audit.LogAsync(
            AuditAction.LoginSuccess,
            notification.UserId,
            null,
            $"Device: {notification.DeviceInfo}",
            notification.IpAddress,
            notification.DeviceInfo,
            true,
            ct);

        // Async fraud assessment
        var assessment = await _fraud.AssessUserAsync(
            notification.UserId, notification.IpAddress, notification.DeviceInfo, ct);

        if (assessment.IsFlagged)
        {
            _logger.LogWarning("Login flagged for user {UserId}: Score={Score}, Reason={Reason}",
                notification.UserId, assessment.Score, assessment.FlagReason);
        }
    }
}
