using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IAuditService _auditService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext dbContext,
        IAuditService auditService,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _dbContext = dbContext;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<AuthTokenResponse>> Handle(LoginCommand request, CancellationToken ct)
    {
        // 1. Find user
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || user.IsDeleted || !user.IsActive)
        {
            await _auditService.LogAsync(AuditAction.LoginFailed, null, request.Email,
                "User not found or inactive", request.IpAddress, request.UserAgent, false, ct);
            return Result<AuthTokenResponse>.Failure("Invalid email or password.");
        }

        // 2. Check lockout
        if (await _userManager.IsLockedOutAsync(user))
        {
            await _auditService.LogAsync(AuditAction.LoginFailed, user.Id, user.Email,
                "Account locked out", request.IpAddress, request.UserAgent, false, ct);
            return Result<AuthTokenResponse>.Failure("Account is locked. Try again later.");
        }

        // 3. Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            await _auditService.LogAsync(AuditAction.LoginFailed, user.Id, user.Email,
                "Invalid password", request.IpAddress, request.UserAgent, false, ct);
            return Result<AuthTokenResponse>.Failure("Invalid email or password.");
        }

        // 4. Reset lockout on success
        await _userManager.ResetAccessFailedCountAsync(user);

        // 5. Create session (device tracking)
        var session = new UserSession
        {
            UserId = user.Id,
            DeviceInfo = request.DeviceInfo ?? "Unknown",
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
        };
        _dbContext.UserSessions.Add(session);

        // 6. Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshTokenPlain = _jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = _jwtTokenService.HashToken(refreshTokenPlain);

        var refreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            SessionId = session.Id,
            TokenFamily = Guid.NewGuid(), // New family for new login
        };
        _dbContext.RefreshTokens.Add(refreshToken);

        await _dbContext.SaveChangesAsync(ct);

        // 7. Audit
        await _auditService.LogAsync(AuditAction.LoginSuccess, user.Id, user.Email,
            $"Device: {request.DeviceInfo}", request.IpAddress, request.UserAgent, true, ct);

        _logger.LogInformation("User {Email} logged in from {Ip}", user.Email, request.IpAddress);

        // Access token returned in body; refresh token set as httpOnly cookie by controller
        return Result<AuthTokenResponse>.Success(new AuthTokenResponse(
            AccessToken: accessToken,
            RefreshToken: refreshTokenPlain,
            ExpiresAt: DateTime.UtcNow.AddMinutes(15).ToString("o")
        ));
    }
}
