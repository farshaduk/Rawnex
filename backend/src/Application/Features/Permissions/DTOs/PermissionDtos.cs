namespace Rawnex.Application.Features.Permissions.DTOs;

public record PermissionDto(
    Guid Id,
    string Resource,
    string Action,
    string Key,
    string? Description,
    bool IsSystem
);

public record RoleWithPermissionsDto(
    Guid RoleId,
    string RoleName,
    string? Description,
    bool IsSystemRole,
    IList<PermissionDto> Permissions
);

public record UserPermissionDto(
    Guid PermissionId,
    string PermissionKey,
    string? Description,
    bool IsGranted,
    string Source // "Role:{roleName}" or "Direct"
);
