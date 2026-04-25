using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class DisputeFiledEventHandler : INotificationHandler<DisputeFiledEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly IEmailService _email;
    private readonly ILogger<DisputeFiledEventHandler> _logger;

    public DisputeFiledEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        IEmailService email,
        ILogger<DisputeFiledEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _email = email;
        _logger = logger;
    }

    public async Task Handle(DisputeFiledEvent notification, CancellationToken ct)
    {
        var dispute = await _db.Disputes.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == notification.DisputeId, ct);
        if (dispute is null) return;

        // Notify the counterparty company
        await _notification.SendToCompanyAsync(
            dispute.AgainstCompanyId,
            "Dispute Filed Against You",
            $"A dispute has been filed regarding order. Please review and respond.",
            NotificationType.DisputeUpdate,
            NotificationPriority.Urgent,
            $"/disputes/{notification.DisputeId}",
            ct);

        // Notify filing company that dispute was submitted
        await _notification.SendToCompanyAsync(
            notification.FiledByCompanyId,
            "Dispute Submitted",
            "Your dispute has been submitted and is under review.",
            NotificationType.DisputeUpdate,
            NotificationPriority.Normal,
            $"/disputes/{notification.DisputeId}",
            ct);

        _logger.LogInformation("Dispute {DisputeId} filed, notifications sent to both parties", notification.DisputeId);
    }
}
