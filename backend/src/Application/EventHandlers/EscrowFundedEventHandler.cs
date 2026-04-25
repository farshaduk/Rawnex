using MediatR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class EscrowFundedEventHandler : INotificationHandler<EscrowFundedEvent>
{
    private readonly INotificationService _notification;
    private readonly IApplicationDbContext _db;
    private readonly ILogger<EscrowFundedEventHandler> _logger;

    public EscrowFundedEventHandler(
        INotificationService notification,
        IApplicationDbContext db,
        ILogger<EscrowFundedEventHandler> logger)
    {
        _notification = notification;
        _db = db;
        _logger = logger;
    }

    public async Task Handle(EscrowFundedEvent notification, CancellationToken ct)
    {
        var escrow = await _db.EscrowAccounts.FindAsync(new object[] { notification.EscrowAccountId }, ct);
        if (escrow is null) return;

        // Notify seller that escrow has been funded
        await _notification.SendToCompanyAsync(
            escrow.SellerCompanyId,
            "Escrow Funded",
            $"The escrow for your order has been fully funded ({notification.Amount:N2}). You may proceed with fulfillment.",
            NotificationType.PaymentUpdate,
            NotificationPriority.High,
            $"/orders/{notification.PurchaseOrderId}",
            ct);

        // Notify buyer that funding was successful
        await _notification.SendToCompanyAsync(
            escrow.BuyerCompanyId,
            "Escrow Funding Confirmed",
            $"Your escrow payment of {notification.Amount:N2} has been confirmed.",
            NotificationType.PaymentUpdate,
            NotificationPriority.Normal,
            $"/orders/{notification.PurchaseOrderId}",
            ct);

        _logger.LogInformation("Escrow {EscrowId} funded, notifications sent", notification.EscrowAccountId);
    }
}
