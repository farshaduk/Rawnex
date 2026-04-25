namespace Rawnex.Application.Features.Auth.DTOs;

public record AuthTokenResponse(
    string AccessToken,
    string RefreshToken,
    string ExpiresAt,
    bool RequiresMfa = false,
    string? MfaToken = null
);
