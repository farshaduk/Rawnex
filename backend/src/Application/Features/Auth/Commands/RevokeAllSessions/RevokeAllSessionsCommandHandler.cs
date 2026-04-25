using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.RevokeAllSessions;

public class RevokeAllSessionsCommandHandler : IRequestHandler<RevokeAllSessionsCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public RevokeAllSessionsCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(RevokeAllSessionsCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId is null)
            return Result.Failure("Not authenticated.");

        var userId = _currentUser.UserId.Value;

        // Revoke all active sessions
        var sessions = await _dbContext.UserSessions
            .Where(s => s.UserId == userId && !s.IsRevoked)
            .ToListAsync(ct);

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = DateTime.UtcNow;
        }

        // Revoke all active refresh tokens
        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ConsumedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = "All sessions revoked by user";
        }

        await _dbContext.SaveChangesAsync(ct);

        await _auditService.LogAsync(AuditAction.AllSessionsRevoked, userId, _currentUser.Email,
            $"Revoked {sessions.Count} sessions, {tokens.Count} tokens",
            request.IpAddress, request.UserAgent, true, ct);

        return Result.Success();
    }
}
