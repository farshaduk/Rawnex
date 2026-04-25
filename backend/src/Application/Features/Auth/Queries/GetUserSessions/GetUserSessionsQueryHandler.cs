using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;

namespace Rawnex.Application.Features.Auth.Queries.GetUserSessions;

/// <summary>
/// Hybrid CQRS read side — queries DbContext directly, no repository.
/// </summary>
public class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, Result<List<SessionDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;

    public GetUserSessionsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<Result<List<SessionDto>>> Handle(GetUserSessionsQuery request, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Result<List<SessionDto>>.Failure("Not authenticated.");

        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == _currentUser.UserId.Value && !s.IsRevoked)
            .OrderByDescending(s => s.LastActivityAt)
            .Select(s => new SessionDto(
                s.Id,
                s.DeviceInfo,
                s.IpAddress,
                s.CreatedAt,
                s.LastActivityAt,
                false // Will be set in controller based on current session
            ))
            .ToListAsync(ct);

        return Result<List<SessionDto>>.Success(sessions);
    }
}
