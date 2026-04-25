using MediatR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEmailService _email;
    private readonly INotificationService _notification;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(
        IEmailService email,
        INotificationService notification,
        ILogger<UserRegisteredEventHandler> logger)
    {
        _email = email;
        _notification = notification;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken ct)
    {
        await _email.SendTemplateAsync(
            notification.Email,
            "welcome",
            new Dictionary<string, string>
            {
                ["email"] = notification.Email,
            },
            ct);

        await _notification.SendAsync(
            notification.UserId,
            null,
            "Welcome to Rawnex!",
            "Your account has been created. Complete your company profile to start trading.",
            NotificationType.System,
            NotificationPriority.Normal,
            "/profile/setup",
            null,
            ct);

        _logger.LogInformation("Welcome email and notification sent to {Email}", notification.Email);
    }
}
