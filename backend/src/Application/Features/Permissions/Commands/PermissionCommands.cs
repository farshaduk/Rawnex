using MediatR;
using Rawnex.Application.Common.Models;

namespace Rawnex.Application.Features.Permissions.Commands;

// ---- Assign permissions to a role ----
public record AssignPermissionsToRoleCommand(
    Guid RoleId,
    IList<Guid> PermissionIds
) : IRequest<Result>;

// ---- Remove permissions from a role ----
public record RevokePermissionsFromRoleCommand(
    Guid RoleId,
    IList<Guid> PermissionIds
) : IRequest<Result>;

// ---- Grant a permission directly to a user ----
public record GrantPermissionToUserCommand(
    Guid UserId,
    Guid PermissionId,
    bool IsGranted = true // false = explicit deny
) : IRequest<Result>;

// ---- Revoke a direct user permission (remove the override) ----
public record RevokeUserPermissionCommand(
    Guid UserId,
    Guid PermissionId
) : IRequest<Result>;
