using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Notifications.DTOs;

public record NotificationDto(
    Guid Id,
    NotificationType Type,
    NotificationPriority Priority,
    string Title,
    string? Message,
    string? ActionUrl,
    bool IsRead,
    DateTime CreatedAt
);

public record NotificationPreferenceDto(
    Guid Id,
    NotificationType Type,
    bool InApp,
    bool Email,
    bool Sms,
    bool Push
);
