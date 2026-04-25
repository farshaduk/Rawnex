using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Permissions.DTOs;

namespace Rawnex.Application.Features.Permissions.Queries;

public class GetAllPermissionsQueryHandler : IRequestHandler<GetAllPermissionsQuery, Result<IList<PermissionDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAllPermissionsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<IList<PermissionDto>>> Handle(GetAllPermissionsQuery request, CancellationToken ct)
    {
        var permissions = await _db.Permissions
            .OrderBy(p => p.Resource).ThenBy(p => p.Action)
            .Select(p => new PermissionDto(
                p.Id, p.Resource, p.Action.ToString().ToLower(), p.Key, p.Description, p.IsSystem))
            .ToListAsync(ct);

        return Result<IList<PermissionDto>>.Success(permissions);
    }
}

public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQuery, Result<RoleWithPermissionsDto>>
{
    private readonly IApplicationDbContext _db;

    public GetRolePermissionsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<RoleWithPermissionsDto>> Handle(GetRolePermissionsQuery request, CancellationToken ct)
    {
        var role = await _db.Roles
            .Where(r => r.Id == request.RoleId)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.IsSystemRole,
                Permissions = _db.RolePermissions
                    .Where(rp => rp.RoleId == r.Id)
                    .Select(rp => rp.Permission)
                    .Select(p => new PermissionDto(
                        p.Id, p.Resource, p.Action.ToString().ToLower(), p.Key, p.Description, p.IsSystem))
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (role is null)
            return Result<RoleWithPermissionsDto>.Failure("Role not found.");

        return Result<RoleWithPermissionsDto>.Success(
            new RoleWithPermissionsDto(role.Id, role.Name!, role.Description, role.IsSystemRole, role.Permissions));
    }
}

public class GetUserEffectivePermissionsQueryHandler : IRequestHandler<GetUserEffectivePermissionsQuery, Result<IList<UserPermissionDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetUserEffectivePermissionsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<IList<UserPermissionDto>>> Handle(GetUserEffectivePermissionsQuery request, CancellationToken ct)
    {
        var userId = request.UserId;

        // 1. Get role-based permissions
        var rolePermissions = await (
            from ur in _db.UserRoles.Where(ur => ur.UserId == userId)
            join rp in _db.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _db.Permissions on rp.PermissionId equals p.Id
            join r in _db.Roles on ur.RoleId equals r.Id
            select new { Permission = p, RoleName = r.Name }
        ).ToListAsync(ct);

        // 2. Get direct user permissions (overrides)
        var directPermissions = await _db.UserPermissions
            .Where(up => up.UserId == userId)
            .Include(up => up.Permission)
            .ToListAsync(ct);

        var directLookup = directPermissions.ToDictionary(dp => dp.PermissionId);

        var result = new List<UserPermissionDto>();

        // Add role permissions (unless overridden by direct denial)
        foreach (var rp in rolePermissions)
        {
            if (directLookup.ContainsKey(rp.Permission.Id))
                continue; // Will be handled in direct section

            result.Add(new UserPermissionDto(
                rp.Permission.Id, rp.Permission.Key, rp.Permission.Description,
                IsGranted: true, Source: $"Role:{rp.RoleName}"));
        }

        // Add direct permissions
        foreach (var dp in directPermissions)
        {
            result.Add(new UserPermissionDto(
                dp.PermissionId, dp.Permission.Key, dp.Permission.Description,
                dp.IsGranted, Source: "Direct"));
        }

        return Result<IList<UserPermissionDto>>.Success(
            result.OrderBy(p => p.PermissionKey).ToList());
    }
}
