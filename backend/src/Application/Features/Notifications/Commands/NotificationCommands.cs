using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Notifications.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Notifications.Commands;

public record MarkNotificationReadCommand(Guid NotificationId) : IRequest<Result>;
public record MarkAllNotificationsReadCommand : IRequest<Result>;

public record UpdateNotificationPreferenceCommand(
    NotificationType Type,
    bool InApp,
    bool Email,
    bool Sms,
    bool Push
) : IRequest<Result>;
