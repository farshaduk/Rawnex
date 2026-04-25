using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class ShipmentStatusChangedEventHandler : INotificationHandler<ShipmentStatusChangedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly IRealTimeNotifier _realTime;
    private readonly IWebhookDispatcher _webhook;
    private readonly ILogger<ShipmentStatusChangedEventHandler> _logger;

    public ShipmentStatusChangedEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        IRealTimeNotifier realTime,
        IWebhookDispatcher webhook,
        ILogger<ShipmentStatusChangedEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _realTime = realTime;
        _webhook = webhook;
        _logger = logger;
    }

    public async Task Handle(ShipmentStatusChangedEvent notification, CancellationToken ct)
    {
        var shipment = await _db.Shipments.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == notification.ShipmentId, ct);
        if (shipment is null) return;

        var message = $"Shipment {shipment.ShipmentNumber} status: {notification.OldStatus} → {notification.NewStatus}";

        // Notify buyer and seller
        await _notification.SendToCompanyAsync(
            shipment.BuyerCompanyId,
            "Shipment Status Updated",
            message,
            NotificationType.ShipmentUpdate,
            NotificationPriority.High,
            $"/shipments/{notification.ShipmentId}",
            ct);

        await _notification.SendToCompanyAsync(
            shipment.SellerCompanyId,
            "Shipment Status Updated",
            message,
            NotificationType.ShipmentUpdate,
            NotificationPriority.Normal,
            $"/shipments/{notification.ShipmentId}",
            ct);

        // Get user IDs for real-time updates
        var buyerUserId = await _db.CompanyMembers
            .Where(m => m.CompanyId == shipment.BuyerCompanyId)
            .Select(m => m.UserId)
            .FirstOrDefaultAsync(ct);

        var sellerUserId = await _db.CompanyMembers
            .Where(m => m.CompanyId == shipment.SellerCompanyId)
            .Select(m => m.UserId)
            .FirstOrDefaultAsync(ct);

        await _realTime.SendShipmentUpdateAsync(
            notification.ShipmentId,
            buyerUserId,
            sellerUserId,
            new
            {
                notification.ShipmentId,
                notification.OldStatus,
                notification.NewStatus,
                Timestamp = DateTime.UtcNow
            },
            ct);

        // Dispatch webhook
        await _webhook.DispatchToAllSubscribersAsync(
            shipment.TenantId,
            "shipment.status_changed",
            new
            {
                notification.ShipmentId,
                notification.OldStatus,
                notification.NewStatus,
                Timestamp = DateTime.UtcNow
            },
            ct);

        _logger.LogInformation("Shipment {ShipmentId} status changed: {Old} → {New}",
            notification.ShipmentId, notification.OldStatus, notification.NewStatus);
    }
}
