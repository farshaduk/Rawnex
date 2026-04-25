using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly IRealTimeNotifier _realTime;
    private readonly IWebhookDispatcher _webhook;
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;

    public OrderStatusChangedEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        IRealTimeNotifier realTime,
        IWebhookDispatcher webhook,
        ILogger<OrderStatusChangedEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _realTime = realTime;
        _webhook = webhook;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == notification.PurchaseOrderId, ct);
        if (order is null) return;

        var message = $"Order status changed from {notification.OldStatus} to {notification.NewStatus}.";

        // Notify buyer
        await _notification.SendToCompanyAsync(
            order.BuyerCompanyId,
            "Order Status Updated",
            message,
            NotificationType.OrderUpdate,
            NotificationPriority.High,
            $"/orders/{notification.PurchaseOrderId}",
            ct);

        // Notify seller
        await _notification.SendToCompanyAsync(
            order.SellerCompanyId,
            "Order Status Updated",
            message,
            NotificationType.OrderUpdate,
            NotificationPriority.High,
            $"/orders/{notification.PurchaseOrderId}",
            ct);

        // Dispatch webhook
        await _webhook.DispatchToAllSubscribersAsync(
            order.TenantId,
            "order.status_changed",
            new
            {
                notification.PurchaseOrderId,
                notification.OldStatus,
                notification.NewStatus,
                Timestamp = DateTime.UtcNow
            },
            ct);

        _logger.LogInformation("Order {OrderId} status changed: {Old} → {New}",
            notification.PurchaseOrderId, notification.OldStatus, notification.NewStatus);
    }
}
