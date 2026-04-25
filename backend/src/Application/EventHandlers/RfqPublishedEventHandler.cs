using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class RfqPublishedEventHandler : INotificationHandler<RfqPublishedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly INotificationService _notification;
    private readonly IEmailService _email;
    private readonly ILogger<RfqPublishedEventHandler> _logger;

    public RfqPublishedEventHandler(
        IApplicationDbContext db,
        INotificationService notification,
        IEmailService email,
        ILogger<RfqPublishedEventHandler> logger)
    {
        _db = db;
        _notification = notification;
        _email = email;
        _logger = logger;
    }

    public async Task Handle(RfqPublishedEvent notification, CancellationToken ct)
    {
        var rfq = await _db.Rfqs.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == notification.RfqId, ct);
        if (rfq is null) return;

        // Notify invited suppliers if it's invite-only
        if (rfq.Visibility == RfqVisibility.InviteOnly || rfq.Visibility == RfqVisibility.Private)
        {
            var invitedCompanyIds = await _db.RfqInvitations
                .Where(i => i.RfqId == notification.RfqId)
                .Select(i => i.SellerCompanyId)
                .ToListAsync(ct);

            foreach (var companyId in invitedCompanyIds)
            {
                await _notification.SendToCompanyAsync(
                    companyId,
                    "New RFQ Invitation",
                    $"You have been invited to respond to an RFQ. Deadline: {rfq.ResponseDeadline:yyyy-MM-dd}",
                    NotificationType.RfqReceived,
                    NotificationPriority.High,
                    $"/rfqs/{notification.RfqId}",
                    ct);
            }
        }

        _logger.LogInformation("RFQ {RfqId} published, notifications sent", notification.RfqId);
    }
}
