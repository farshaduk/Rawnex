using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Permissions.DTOs;

namespace Rawnex.Application.Features.Permissions.Queries;

// ---- Get all system permissions ----
public record GetAllPermissionsQuery : IRequest<Result<IList<PermissionDto>>>;

// ---- Get a role's permissions ----
public record GetRolePermissionsQuery(Guid RoleId) : IRequest<Result<RoleWithPermissionsDto>>;

// ---- Get a user's effective permissions ----
public record GetUserEffectivePermissionsQuery(Guid UserId) : IRequest<Result<IList<UserPermissionDto>>>;
