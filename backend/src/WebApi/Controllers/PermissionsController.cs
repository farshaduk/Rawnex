using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Permissions.Commands;
using Rawnex.Application.Features.Permissions.Queries;

namespace Rawnex.WebApi.Controllers;

/// <summary>
/// API for dynamic permission management.
/// Only users with "permissions.manage" can access these endpoints.
/// </summary>
[Authorize]
public class PermissionsController : BaseApiController
{
    /// <summary>
    /// Get all system permissions.
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "Permission:permissions.read")]
    public async Task<IActionResult> GetAll()
    {
        var result = await Mediator.Send(new GetAllPermissionsQuery());
        return Ok(result.Value);
    }

    /// <summary>
    /// Get permissions assigned to a role.
    /// </summary>
    [HttpGet("roles/{roleId:guid}")]
    [Authorize(Policy = "Permission:permissions.read")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        var result = await Mediator.Send(new GetRolePermissionsQuery(roleId));
        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Get effective permissions for a user (role + direct).
    /// </summary>
    [HttpGet("users/{userId:guid}")]
    [Authorize(Policy = "Permission:permissions.read")]
    public async Task<IActionResult> GetUserPermissions(Guid userId)
    {
        var result = await Mediator.Send(new GetUserEffectivePermissionsQuery(userId));
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    /// <summary>
    /// Assign permissions to a role.
    /// </summary>
    [HttpPost("roles/{roleId:guid}/assign")]
    [Authorize(Policy = "Permission:permissions.manage")]
    public async Task<IActionResult> AssignToRole(Guid roleId, [FromBody] PermissionIdsRequest request)
    {
        var result = await Mediator.Send(new AssignPermissionsToRoleCommand(roleId, request.PermissionIds));
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(new { message = "Permissions assigned to role." });
    }

    /// <summary>
    /// Revoke permissions from a role.
    /// </summary>
    [HttpPost("roles/{roleId:guid}/revoke")]
    [Authorize(Policy = "Permission:permissions.manage")]
    public async Task<IActionResult> RevokeFromRole(Guid roleId, [FromBody] PermissionIdsRequest request)
    {
        var result = await Mediator.Send(new RevokePermissionsFromRoleCommand(roleId, request.PermissionIds));
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(new { message = "Permissions revoked from role." });
    }

    /// <summary>
    /// Grant or deny a permission directly to a user.
    /// </summary>
    [HttpPost("users/{userId:guid}/grant")]
    [Authorize(Policy = "Permission:permissions.manage")]
    public async Task<IActionResult> GrantToUser(Guid userId, [FromBody] UserPermissionGrantRequest request)
    {
        var result = await Mediator.Send(new GrantPermissionToUserCommand(userId, request.PermissionId, request.IsGranted));
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(new { message = request.IsGranted ? "Permission granted." : "Permission denied." });
    }

    /// <summary>
    /// Remove a direct permission override from a user.
    /// </summary>
    [HttpDelete("users/{userId:guid}/revoke/{permissionId:guid}")]
    [Authorize(Policy = "Permission:permissions.manage")]
    public async Task<IActionResult> RevokeFromUser(Guid userId, Guid permissionId)
    {
        var result = await Mediator.Send(new RevokeUserPermissionCommand(userId, permissionId));
        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });
        return Ok(new { message = "Direct permission removed." });
    }
}

// ---- Request DTOs (API boundary) ----
public record PermissionIdsRequest(IList<Guid> PermissionIds);
public record UserPermissionGrantRequest(Guid PermissionId, bool IsGranted = true);
