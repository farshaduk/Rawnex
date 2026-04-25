using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.EventHandlers;

public class TokenReuseDetectedEventHandler : INotificationHandler<TokenReuseDetectedEvent>
{
    private readonly IApplicationDbContext _db;
    private readonly IAuditService _audit;
    private readonly IEmailService _email;
    private readonly ILogger<TokenReuseDetectedEventHandler> _logger;

    public TokenReuseDetectedEventHandler(
        IApplicationDbContext db,
        IAuditService audit,
        IEmailService email,
        ILogger<TokenReuseDetectedEventHandler> logger)
    {
        _db = db;
        _audit = audit;
        _email = email;
        _logger = logger;
    }

    public async Task Handle(TokenReuseDetectedEvent notification, CancellationToken ct)
    {
        _logger.LogWarning("Token reuse detected for user {UserId}, family {Family}, IP {Ip}",
            notification.UserId, notification.TokenFamily, notification.IpAddress);

        // Revoke all refresh tokens for this user
        var tokens = await _db.RefreshTokens
            .Where(t => t.UserId == notification.UserId && !t.IsRevoked)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedReason = "Token reuse detected — all tokens revoked";
        }

        // Revoke all sessions
        var sessions = await _db.UserSessions
            .Where(s => s.UserId == notification.UserId && !s.IsRevoked)
            .ToListAsync(ct);

        foreach (var session in sessions)
        {
            session.IsRevoked = true;
            session.RevokedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);

        await _audit.LogAsync(
            AuditAction.TokenReuseDetected,
            notification.UserId,
            null,
            $"Token family: {notification.TokenFamily}. All sessions revoked.",
            notification.IpAddress,
            null,
            true,
            ct);

        // Notify user via email
        var user = await _db.Users.FindAsync(new object[] { notification.UserId }, ct);
        if (user?.Email is not null)
        {
            await _email.SendAsync(
                user.Email,
                "Security Alert — Suspicious Token Activity",
                $"<p>We detected suspicious activity on your account. All your active sessions have been revoked for security. Please log in again and change your password.</p><p>IP Address: {notification.IpAddress}</p>",
                ct);
        }
    }
}
