using System.Text;
using System.Text.Encodings.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Auth.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Auth.Commands.Mfa;

public class EnableMfaCommandHandler : IRequestHandler<EnableMfaCommand, Result<MfaSetupResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;
    private readonly UrlEncoder _urlEncoder;

    public EnableMfaCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser, UrlEncoder urlEncoder)
    {
        _userManager = userManager;
        _currentUser = currentUser;
        _urlEncoder = urlEncoder;
    }

    public async Task<Result<MfaSetupResponse>> Handle(EnableMfaCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null) return Result<MfaSetupResponse>.Failure("User not found.");

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        var uri = GenerateQrCodeUri(user.Email!, key!);
        return Result<MfaSetupResponse>.Success(new MfaSetupResponse(FormatKey(key!), uri));
    }

    private string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        int currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }
        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }
        return result.ToString().ToLowerInvariant();
    }

    private string GenerateQrCodeUri(string email, string key)
    {
        return $"otpauth://totp/{_urlEncoder.Encode("Rawnex")}:{_urlEncoder.Encode(email)}?secret={key}&issuer={_urlEncoder.Encode("Rawnex")}&digits=6";
    }
}

public class VerifyMfaSetupCommandHandler : IRequestHandler<VerifyMfaSetupCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public VerifyMfaSetupCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(VerifyMfaSetupCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null) return Result.Failure("User not found.");

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (!isValid) return Result.Failure("Invalid verification code.");

        await _userManager.SetTwoFactorEnabledAsync(user, true);
        return Result.Success();
    }
}

public class DisableMfaCommandHandler : IRequestHandler<DisableMfaCommand, Result>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public DisableMfaCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DisableMfaCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null) return Result.Failure("User not found.");

        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (!isValid) return Result.Failure("Invalid verification code.");

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);
        return Result.Success();
    }
}

public class VerifyMfaLoginCommandHandler : IRequestHandler<VerifyMfaLoginCommand, Result<AuthTokenResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ILogger<VerifyMfaLoginCommandHandler> _logger;

    public VerifyMfaLoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext context,
        IAuditService auditService,
        ILogger<VerifyMfaLoginCommandHandler> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _context = context;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<AuthTokenResponse>> Handle(VerifyMfaLoginCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null) return Result<AuthTokenResponse>.Failure("User not found.");

        // Try authenticator code first, then recovery code
        var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, request.Code);
        if (!isValid)
        {
            // Try as a recovery code
            var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(user, request.Code);
            if (!result.Succeeded)
            {
                await _auditService.LogAsync(AuditAction.LoginFailed, user.Id, user.Email!, "MFA verification failed", request.IpAddress, request.UserAgent, false);
                return Result<AuthTokenResponse>.Failure("Invalid MFA code.");
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email!, (IList<string>)roles);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        var hashedRefreshToken = _jwtTokenService.HashToken(refreshToken);

        var tokenEntity = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            TokenFamily = Guid.NewGuid(),
        };
        _context.RefreshTokens.Add(tokenEntity);

        var session = new UserSession
        {
            UserId = user.Id,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            DeviceInfo = request.DeviceInfo ?? "Unknown",
        };
        _context.UserSessions.Add(session);
        await _context.SaveChangesAsync(ct);

        await _auditService.LogAsync(AuditAction.LoginSuccess, user.Id, user.Email!, "MFA login successful", request.IpAddress, request.UserAgent, true);

        return Result<AuthTokenResponse>.Success(new AuthTokenResponse(accessToken, refreshToken, DateTime.UtcNow.AddMinutes(15).ToString("o")));
    }
}

public class GenerateRecoveryCodesCommandHandler : IRequestHandler<GenerateRecoveryCodesCommand, Result<IList<string>>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GenerateRecoveryCodesCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<Result<IList<string>>> Handle(GenerateRecoveryCodesCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user is null) return Result<IList<string>>.Failure("User not found.");

        if (!await _userManager.GetTwoFactorEnabledAsync(user))
            return Result<IList<string>>.Failure("MFA is not enabled.");

        var codes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        return Result<IList<string>>.Success(codes!.ToList());
    }
}
