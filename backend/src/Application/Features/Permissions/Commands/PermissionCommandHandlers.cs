using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Permissions.Commands;

// ---- Assign permissions to a role ----
public class AssignPermissionsToRoleHandler : IRequestHandler<AssignPermissionsToRoleCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public AssignPermissionsToRoleHandler(
        IApplicationDbContext db,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        IAuditService audit)
    {
        _db = db;
        _permissionService = permissionService;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<Result> Handle(AssignPermissionsToRoleCommand request, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (role is null)
            return Result.Failure("Role not found.");

        // Validate all permission IDs exist
        var validPermissionIds = await _db.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        if (validPermissionIds.Count != request.PermissionIds.Count)
            return Result.Failure("One or more permission IDs are invalid.");

        // Get existing role permissions to avoid duplicates
        var existingIds = await _db.RolePermissions
            .Where(rp => rp.RoleId == request.RoleId && request.PermissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToHashSetAsync(ct);

        var newPermissions = request.PermissionIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new RolePermission
            {
                RoleId = request.RoleId,
                PermissionId = id,
                GrantedBy = _currentUser.Email
            })
            .ToList();

        if (newPermissions.Count > 0)
        {
            _db.RolePermissions.AddRange(newPermissions);
            await _db.SaveChangesAsync(ct);
        }

        // Invalidate cache for all users in this role
        await _permissionService.InvalidateRoleCacheAsync(request.RoleId, ct);

        await _audit.LogAsync(AuditAction.PermissionGrantedToRole, null, _currentUser.Email,
            $"{newPermissions.Count} permissions assigned to role '{role.Name}'",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}

// ---- Remove permissions from a role ----
public class RevokePermissionsFromRoleHandler : IRequestHandler<RevokePermissionsFromRoleCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public RevokePermissionsFromRoleHandler(
        IApplicationDbContext db,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        IAuditService audit)
    {
        _db = db;
        _permissionService = permissionService;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<Result> Handle(RevokePermissionsFromRoleCommand request, CancellationToken ct)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == request.RoleId, ct);
        if (role is null)
            return Result.Failure("Role not found.");

        var toRemove = await _db.RolePermissions
            .Where(rp => rp.RoleId == request.RoleId && request.PermissionIds.Contains(rp.PermissionId))
            .ToListAsync(ct);

        if (toRemove.Count > 0)
        {
            _db.RolePermissions.RemoveRange(toRemove);
            await _db.SaveChangesAsync(ct);
        }

        await _permissionService.InvalidateRoleCacheAsync(request.RoleId, ct);

        await _audit.LogAsync(AuditAction.PermissionRevokedFromRole, null, _currentUser.Email,
            $"{toRemove.Count} permissions revoked from role '{role.Name}'",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}

// ---- Grant a permission directly to a user ----
public class GrantPermissionToUserHandler : IRequestHandler<GrantPermissionToUserCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public GrantPermissionToUserHandler(
        IApplicationDbContext db,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        IAuditService audit)
    {
        _db = db;
        _permissionService = permissionService;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<Result> Handle(GrantPermissionToUserCommand request, CancellationToken ct)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId && !u.IsDeleted, ct);
        if (!userExists)
            return Result.Failure("User not found.");

        var permissionExists = await _db.Permissions.AnyAsync(p => p.Id == request.PermissionId, ct);
        if (!permissionExists)
            return Result.Failure("Permission not found.");

        // Upsert: update if exists, insert if not
        var existing = await _db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId, ct);

        if (existing is not null)
        {
            existing.IsGranted = request.IsGranted;
            existing.GrantedBy = _currentUser.Email;
            existing.GrantedAt = DateTime.UtcNow;
        }
        else
        {
            _db.UserPermissions.Add(new UserPermission
            {
                UserId = request.UserId,
                PermissionId = request.PermissionId,
                IsGranted = request.IsGranted,
                GrantedBy = _currentUser.Email
            });
        }

        await _db.SaveChangesAsync(ct);
        _permissionService.InvalidateUserCache(request.UserId);

        var action = request.IsGranted ? "granted to" : "denied for";
        await _audit.LogAsync(AuditAction.PermissionGrantedToUser, request.UserId, _currentUser.Email,
            $"Permission {request.PermissionId} {action} user {request.UserId}",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}

// ---- Revoke a direct user permission ----
public class RevokeUserPermissionHandler : IRequestHandler<RevokeUserPermissionCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IPermissionService _permissionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IAuditService _audit;

    public RevokeUserPermissionHandler(
        IApplicationDbContext db,
        IPermissionService permissionService,
        ICurrentUserService currentUser,
        IAuditService audit)
    {
        _db = db;
        _permissionService = permissionService;
        _currentUser = currentUser;
        _audit = audit;
    }

    public async Task<Result> Handle(RevokeUserPermissionCommand request, CancellationToken ct)
    {
        var existing = await _db.UserPermissions
            .FirstOrDefaultAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId, ct);

        if (existing is null)
            return Result.Failure("Direct permission not found for this user.");

        _db.UserPermissions.Remove(existing);
        await _db.SaveChangesAsync(ct);
        _permissionService.InvalidateUserCache(request.UserId);

        await _audit.LogAsync(AuditAction.PermissionRevokedFromUser, request.UserId, _currentUser.Email,
            $"Direct permission {request.PermissionId} revoked from user {request.UserId}",
            _currentUser.IpAddress, _currentUser.UserAgent, true, ct);

        return Result.Success();
    }
}
