using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Auth.Commands.AssignRole;
using Rawnex.Application.Features.Auth.Commands.Login;
using Rawnex.Application.Features.Auth.Commands.Logout;
using Rawnex.Application.Features.Auth.Commands.Mfa;
using Rawnex.Application.Features.Auth.Commands.RefreshToken;
using Rawnex.Application.Features.Auth.Commands.Register;
using Rawnex.Application.Features.Auth.Commands.RemoveRole;
using Rawnex.Application.Features.Auth.Commands.RevokeAllSessions;
using Rawnex.Application.Features.Auth.Queries.GetCurrentUser;
using Rawnex.Application.Features.Auth.Queries.GetUserSessions;

namespace Rawnex.WebApi.Controllers;

public class AuthController : BaseApiController
{
    /// <summary>
    /// Authenticate user and return access token + refresh token (httpOnly cookie).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(
            request.Email,
            request.Password,
            request.DeviceInfo,
            IpAddress,
            UserAgentHeader);

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return Unauthorized(new { isSuccess = false, error = result.Error });

        // Set refresh token as httpOnly secure cookie
        SetRefreshTokenCookie(result.Value!.RefreshToken);

        return Ok(new { isSuccess = true, data = result.Value });
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            IpAddress,
            UserAgentHeader);

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { isSuccess = false, errors = result.Errors });

        return Ok(new { isSuccess = true, data = result.Value });
    }

    /// <summary>
    /// Refresh access token using refresh token from httpOnly cookie.
    /// Implements token rotation + reuse detection.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = Request.Cookies["refreshToken"] ?? request.RefreshToken;

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { error = "Refresh token is required." });

        var command = new RefreshTokenCommand(
            request.AccessToken,
            refreshToken,
            IpAddress,
            UserAgentHeader);

        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
        {
            // Clear the cookie on failure
            Response.Cookies.Delete("refreshToken");
            return Unauthorized(new { isSuccess = false, error = result.Error });
        }

        // Set new refresh token cookie
        SetRefreshTokenCookie(result.Value!.RefreshToken);

        return Ok(new { isSuccess = true, data = result.Value });
    }

    /// <summary>
    /// Logout current session — revoke refresh token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            return Ok();

        var command = new LogoutCommand(refreshToken, IpAddress, UserAgentHeader);
        await Mediator.Send(command);

        Response.Cookies.Delete("refreshToken");
        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>
    /// Revoke all sessions for the current user ("log out everywhere").
    /// </summary>
    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<IActionResult> RevokeAllSessions()
    {
        var command = new RevokeAllSessionsCommand(IpAddress, UserAgentHeader);
        var result = await Mediator.Send(command);

        Response.Cookies.Delete("refreshToken");

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = "All sessions revoked." });
    }

    /// <summary>
    /// Get the current authenticated user info.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await Mediator.Send(new GetCurrentUserQuery());

        if (!result.IsSuccess)
            return NotFound(new { isSuccess = false, error = result.Error });

        return Ok(new { isSuccess = true, data = result.Value });
    }

    /// <summary>
    /// Get all active sessions for the current user.
    /// </summary>
    [HttpGet("sessions")]
    [Authorize]
    public async Task<IActionResult> GetSessions()
    {
        var result = await Mediator.Send(new GetUserSessionsQuery());

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Assign a role to a user (Admin only).
    /// </summary>
    [HttpPost("roles/assign")]
    [Authorize(Policy = "Permission:roles.manage")]
    public async Task<IActionResult> AssignRole([FromBody] RoleAssignmentRequest request)
    {
        var command = new AssignRoleCommand(request.UserId, request.RoleName);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = $"Role '{request.RoleName}' assigned successfully." });
    }

    /// <summary>
    /// Remove a role from a user (Admin only).
    /// </summary>
    [HttpPost("roles/remove")]
    [Authorize(Policy = "Permission:roles.manage")]
    public async Task<IActionResult> RemoveRole([FromBody] RoleAssignmentRequest request)
    {
        var command = new RemoveRoleCommand(request.UserId, request.RoleName);
        var result = await Mediator.Send(command);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return Ok(new { message = $"Role '{request.RoleName}' removed successfully." });
    }

    // ---- Private helpers ----

    private void SetRefreshTokenCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
            Path = "/api/auth",
        };
        Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
}

// ---- MFA Controller ----

[Route("api/auth/mfa")]
public class MfaController : BaseApiController
{
    [HttpPost("enable")]
    [Authorize]
    public async Task<IActionResult> EnableMfa()
    {
        var result = await Mediator.Send(new EnableMfaCommand());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    [HttpPost("verify-setup")]
    [Authorize]
    public async Task<IActionResult> VerifyMfaSetup([FromBody] MfaCodeRequest request)
    {
        var result = await Mediator.Send(new VerifyMfaSetupCommand(request.Code));
        return result.IsSuccess ? Ok(new { message = "MFA enabled successfully." }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("disable")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] MfaCodeRequest request)
    {
        var result = await Mediator.Send(new DisableMfaCommand(request.Code));
        return result.IsSuccess ? Ok(new { message = "MFA disabled." }) : BadRequest(new { error = result.Error });
    }

    [HttpPost("verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMfaLogin([FromBody] VerifyMfaLoginRequest request)
    {
        var command = new VerifyMfaLoginCommand(request.UserId, request.Code, request.DeviceInfo,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            Request.Headers.UserAgent.FirstOrDefault());
        var result = await Mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : Unauthorized(new { error = result.Error });
    }

    [HttpPost("recovery-codes")]
    [Authorize]
    public async Task<IActionResult> GenerateRecoveryCodes()
    {
        var result = await Mediator.Send(new GenerateRecoveryCodesCommand());
        return result.IsSuccess ? Ok(new { codes = result.Value }) : BadRequest(new { error = result.Error });
    }
}

// ---- Request DTOs (API boundary) ----

public record LoginRequest(string Email, string Password, string? DeviceInfo);
public record RegisterRequest(string Email, string Password, string FirstName, string LastName);
public record RefreshTokenRequest(string AccessToken, string? RefreshToken);
public record RoleAssignmentRequest(Guid UserId, string RoleName);
public record MfaCodeRequest(string Code);
public record VerifyMfaLoginRequest(Guid UserId, string Code, string? DeviceInfo);
