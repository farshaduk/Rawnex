namespace Rawnex.Application.Common.Interfaces;

public interface INotificationService
{
    Task SendAsync(Guid userId, Guid? tenantId, string title, string? message,
        Domain.Enums.NotificationType type, Domain.Enums.NotificationPriority priority,
        string? actionUrl = null, string? dataJson = null, CancellationToken ct = default);

    Task SendToCompanyAsync(Guid companyId, string title, string? message,
        Domain.Enums.NotificationType type, Domain.Enums.NotificationPriority priority,
        string? actionUrl = null, CancellationToken ct = default);
}
