using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _auditService;

    public LogoutCommandHandler(
        IApplicationDbContext dbContext,
        IJwtTokenService jwtTokenService,
        ICurrentUserService currentUser,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var tokenHash = _jwtTokenService.HashToken(request.RefreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .Include(rt => rt.Session)
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, ct);

        if (storedToken is not null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            storedToken.RevokedReason = "User logout";

            if (storedToken.Session is not null)
            {
                storedToken.Session.IsRevoked = true;
                storedToken.Session.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        await _auditService.LogAsync(AuditAction.Logout, _currentUser.UserId, _currentUser.Email,
            null, request.IpAddress, request.UserAgent, true, ct);

        return Result.Success();
    }
}
