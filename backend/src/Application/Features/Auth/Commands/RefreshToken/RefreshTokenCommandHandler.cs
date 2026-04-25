using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokenResponse>>
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _auditService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IJwtTokenService jwtTokenService,
        IApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IAuditService auditService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _userManager = userManager;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<AuthTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // 1. Validate the expired access token to extract claims
        var principal = _jwtTokenService.ValidateExpiredToken(request.AccessToken);
        if (principal is null)
            return Result<AuthTokenResponse>.Failure("Invalid access token.");

        var userIdClaim = principal.FindFirst("sub")?.Value ?? principal.FindFirst("nameid")?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return Result<AuthTokenResponse>.Failure("Invalid token claims.");

        // 2. Hash the incoming refresh token and find it in DB
        var incomingHash = _jwtTokenService.HashToken(request.RefreshToken);
        var storedToken = await _dbContext.RefreshTokens
            .Include(rt => rt.Session)
            .FirstOrDefaultAsync(rt => rt.TokenHash == incomingHash && rt.UserId == userId, ct);

        if (storedToken is null)
        {
            await _auditService.LogAsync(AuditAction.TokenRefreshFailed, userId, null,
                "Refresh token not found", request.IpAddress, request.UserAgent, false, ct);
            return Result<AuthTokenResponse>.Failure("Invalid refresh token.");
        }

        // 3. REUSE DETECTION — token already consumed? This means stolen token replay!
        if (storedToken.IsConsumed || storedToken.IsRevoked)
        {
            _logger.LogWarning("TOKEN REUSE DETECTED for user {UserId}, family {Family}. Revoking entire family.",
                userId, storedToken.TokenFamily);

            // Revoke ALL tokens in this family
            var familyTokens = await _dbContext.RefreshTokens
                .Where(rt => rt.TokenFamily == storedToken.TokenFamily)
                .ToListAsync(ct);

            foreach (var ft in familyTokens)
            {
                ft.RevokedAt = DateTime.UtcNow;
                ft.RevokedReason = "Token reuse detected — potential theft";
            }

            // Revoke the session too
            if (storedToken.Session is not null)
            {
                storedToken.Session.IsRevoked = true;
                storedToken.Session.RevokedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(ct);

            await _auditService.LogAsync(AuditAction.TokenReuseDetected, userId, null,
                $"Family {storedToken.TokenFamily} revoked", request.IpAddress, request.UserAgent, false, ct);

            return Result<AuthTokenResponse>.Failure("Security violation detected. Please log in again.");
        }

        // 4. Check if token is expired
        if (storedToken.IsExpired)
        {
            await _auditService.LogAsync(AuditAction.TokenRefreshFailed, userId, null,
                "Refresh token expired", request.IpAddress, request.UserAgent, false, ct);
            return Result<AuthTokenResponse>.Failure("Refresh token expired. Please log in again.");
        }

        // 5. Get user
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null || user.IsDeleted || !user.IsActive)
            return Result<AuthTokenResponse>.Failure("User not found or inactive.");

        // 6. ROTATION — consume old token and create new one
        storedToken.ConsumedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var newRefreshTokenPlain = _jwtTokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _jwtTokenService.HashToken(newRefreshTokenPlain);

        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            SessionId = storedToken.SessionId,
            TokenFamily = storedToken.TokenFamily, // SAME family for reuse detection
        };

        storedToken.ReplacedByTokenId = newRefreshToken.Id;

        _dbContext.RefreshTokens.Add(newRefreshToken);

        // Update session activity
        if (storedToken.Session is not null)
        {
            storedToken.Session.LastActivityAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(ct);

        await _auditService.LogAsync(AuditAction.TokenRefreshed, userId, user.Email,
            null, request.IpAddress, request.UserAgent, true, ct);

        return Result<AuthTokenResponse>.Success(new AuthTokenResponse(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshTokenPlain,
            ExpiresAt: DateTime.UtcNow.AddMinutes(15).ToString("o")
        ));
    }
}
