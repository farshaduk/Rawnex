using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Notifications.Commands;

public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken ct)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == _currentUser.UserId, ct);

        if (notification is null) return Result.Failure("Notification not found.");

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken ct)
    {
        var unread = await _db.Notifications
            .Where(n => n.UserId == _currentUser.UserId && !n.IsRead)
            .ToListAsync(ct);

        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class UpdateNotificationPreferenceCommandHandler : IRequestHandler<UpdateNotificationPreferenceCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateNotificationPreferenceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateNotificationPreferenceCommand request, CancellationToken ct)
    {
        var pref = await _db.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId && p.Type == request.Type, ct);

        if (pref is null)
        {
            pref = new NotificationPreference
            {
                UserId = _currentUser.UserId!.Value,
                Type = request.Type,
            };
            _db.NotificationPreferences.Add(pref);
        }

        pref.InApp = request.InApp;
        pref.Email = request.Email;
        pref.Sms = request.Sms;
        pref.Push = request.Push;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
