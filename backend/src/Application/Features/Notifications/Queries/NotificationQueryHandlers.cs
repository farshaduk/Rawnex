using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Notifications.DTOs;

namespace Rawnex.Application.Features.Notifications.Queries;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, Result<PaginatedList<NotificationDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PaginatedList<NotificationDto>>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        var result = await _db.Notifications.AsNoTracking()
            .Where(n => n.UserId == _currentUser.UserId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(
                n.Id, n.Type, n.Priority, n.Title, n.Message,
                n.ActionUrl, n.IsRead, n.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<NotificationDto>>.Success(result);
    }
}

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(GetUnreadCountQuery request, CancellationToken ct)
    {
        var count = await _db.Notifications
            .CountAsync(n => n.UserId == _currentUser.UserId && !n.IsRead, ct);

        return Result<int>.Success(count);
    }
}

public class GetMyPreferencesQueryHandler : IRequestHandler<GetMyPreferencesQuery, Result<List<NotificationPreferenceDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyPreferencesQueryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<List<NotificationPreferenceDto>>> Handle(GetMyPreferencesQuery request, CancellationToken ct)
    {
        var prefs = await _db.NotificationPreferences.AsNoTracking()
            .Where(p => p.UserId == _currentUser.UserId)
            .Select(p => new NotificationPreferenceDto(p.Id, p.Type, p.InApp, p.Email, p.Sms, p.Push))
            .ToListAsync(ct);

        return Result<List<NotificationPreferenceDto>>.Success(prefs);
    }
}
