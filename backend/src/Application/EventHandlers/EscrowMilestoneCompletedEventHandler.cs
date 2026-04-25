using MediatR;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class EscrowMilestoneCompletedEventHandler : INotificationHandler<EscrowMilestoneCompletedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly ILogger<EscrowMilestoneCompletedEventHandler> _logger;

    public EscrowMilestoneCompletedEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        ILogger<EscrowMilestoneCompletedEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _logger = logger;
    }

    public async Task Handle(EscrowMilestoneCompletedEvent notification, CancellationToken ct)
    {
        var escrow = await _db.EscrowAccounts.FindAsync(new object[] { notification.EscrowAccountId }, ct);
        if (escrow is null) return;

        await _notification.SendToCompanyAsync(
            escrow.SellerCompanyId,
            "Milestone Completed — Funds Released",
            $"A milestone has been completed and {notification.ReleasedAmount:N2} has been released from escrow.",
            NotificationType.PaymentUpdate,
            NotificationPriority.High,
            $"/escrow/{notification.EscrowAccountId}",
            ct);

        await _notification.SendToCompanyAsync(
            escrow.BuyerCompanyId,
            "Milestone Completed",
            $"A milestone for your order has been completed. {notification.ReleasedAmount:N2} released to seller.",
            NotificationType.PaymentUpdate,
            NotificationPriority.Normal,
            $"/escrow/{notification.EscrowAccountId}",
            ct);

        _logger.LogInformation("Milestone {MilestoneId} completed, {Amount} released",
            notification.MilestoneId, notification.ReleasedAmount);
    }
}
