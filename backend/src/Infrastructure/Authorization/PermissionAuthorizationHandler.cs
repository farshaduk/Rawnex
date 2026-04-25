using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Authorization;

/// <summary>
/// Checks if the authenticated user has the required permission via the PermissionService.
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(JwtRegisteredClaimNames.Sub)
                          ?? context.User.FindFirst("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return; // Not authenticated — fail silently (returns 403)

        if (await _permissionService.HasPermissionAsync(userId, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
