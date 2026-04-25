using System.Security.Claims;

namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// JWT token generation and validation service.
/// Defined in Application, implemented in Infrastructure.
/// </summary>
public interface IJwtTokenService
{
    string GenerateAccessToken(Guid userId, string email, IList<string> roles);
    string GenerateRefreshToken();
    string HashToken(string token);
    ClaimsPrincipal? ValidateExpiredToken(string token);
}
