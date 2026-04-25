using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IApplicationDbContext db, ILogger<NotificationService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SendAsync(Guid userId, Guid? tenantId, string title, string? message,
        NotificationType type, NotificationPriority priority,
        string? actionUrl = null, string? dataJson = null, CancellationToken ct = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            TenantId = tenantId,
            Title = title,
            Message = message,
            Type = type,
            Priority = priority,
            ActionUrl = actionUrl,
            DataJson = dataJson,
        };

        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("Notification sent to user {UserId}: {Title}", userId, title);
    }

    public async Task SendToCompanyAsync(Guid companyId, string title, string? message,
        NotificationType type, NotificationPriority priority,
        string? actionUrl = null, CancellationToken ct = default)
    {
        var memberUserIds = await _db.CompanyMembers
            .Where(m => m.CompanyId == companyId)
            .Select(m => m.UserId)
            .ToListAsync(ct);

        foreach (var userId in memberUserIds)
        {
            _db.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                Priority = priority,
                ActionUrl = actionUrl,
            });
        }

        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Notification sent to {Count} members of company {CompanyId}: {Title}",
            memberUserIds.Count, companyId, title);
    }
}
