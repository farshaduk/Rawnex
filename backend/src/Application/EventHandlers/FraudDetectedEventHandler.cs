using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class FraudDetectedEventHandler : INotificationHandler<FraudDetectedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly IAuditService _audit;
    private readonly ILogger<FraudDetectedEventHandler> _logger;

    public FraudDetectedEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        IAuditService audit,
        ILogger<FraudDetectedEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _audit = audit;
        _logger = logger;
    }

    public async Task Handle(FraudDetectedEvent notification, CancellationToken ct)
    {
        _logger.LogWarning("Fraud detected: Risk={Risk}, Reason={Reason}, User={UserId}, Company={CompanyId}",
            notification.RiskLevel, notification.Reason, notification.UserId, notification.CompanyId);

        // Notify all admin users
        var adminUserIds = await _db.UserRoles
            .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.NormalizedName })
            .Where(x => x.NormalizedName == "ADMIN" || x.NormalizedName == "SUPERADMIN")
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(ct);

        foreach (var adminId in adminUserIds)
        {
            await _notification.SendAsync(
                adminId,
                null,
                "Fraud Alert",
                $"Risk Level: {notification.RiskLevel}. {notification.Reason}",
                NotificationType.SecurityAlert,
                NotificationPriority.Urgent,
                "/admin/fraud-alerts",
                null,
                ct);
        }

        await _audit.LogAsync(
            AuditAction.SensitiveDataAccessed,
            notification.UserId,
            null,
            $"Fraud detected: {notification.Reason} (Risk: {notification.RiskLevel})",
            null,
            null,
            true,
            ct);
    }
}
