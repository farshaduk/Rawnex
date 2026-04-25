using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Auth.Commands.Mfa;

/// <summary>Enable MFA for the current user. Generates a TOTP secret key and returns a setup URI.</summary>
public record EnableMfaCommand : IRequest<Result<MfaSetupResponse>>;

/// <summary>Verify the TOTP code after enabling MFA, finalizing the setup.</summary>
public record VerifyMfaSetupCommand(string Code) : IRequest<Result>;

/// <summary>Disable MFA for the current user (requires valid code).</summary>
public record DisableMfaCommand(string Code) : IRequest<Result>;

/// <summary>Verify a TOTP code during login (second factor).</summary>
public record VerifyMfaLoginCommand(Guid UserId, string Code, string? DeviceInfo, string? IpAddress, string? UserAgent) : IRequest<Result<Auth.DTOs.AuthTokenResponse>>;

/// <summary>Generate single-use recovery codes for MFA.</summary>
public record GenerateRecoveryCodesCommand : IRequest<Result<IList<string>>>;

public record MfaSetupResponse(string SharedKey, string AuthenticatorUri);
