using MediatR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class CompanyVerifiedEventHandler : INotificationHandler<CompanyVerifiedEvent>
{
    private readonly INotificationService _notification;
    private readonly ILogger<CompanyVerifiedEventHandler> _logger;

    public CompanyVerifiedEventHandler(
        INotificationService notification,
        ILogger<CompanyVerifiedEventHandler> logger)
    {
        _notification = notification;
        _logger = logger;
    }

    public async Task Handle(CompanyVerifiedEvent notification, CancellationToken ct)
    {
        await _notification.SendToCompanyAsync(
            notification.CompanyId,
            "Company Verified!",
            "Your company has been verified. You can now start trading on the platform.",
            NotificationType.System,
            NotificationPriority.High,
            "/dashboard",
            ct);

        _logger.LogInformation("Company {CompanyId} verified, notifications sent", notification.CompanyId);
    }
}
