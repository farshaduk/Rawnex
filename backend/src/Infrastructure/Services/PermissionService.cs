using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

/// <summary>
/// Resolves user permissions from DB with in-memory caching.
/// Cache is invalidated on permission changes — no stale data.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IApplicationDbContext _db;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private static string CacheKey(Guid userId) => $"permissions:{userId}";

    public PermissionService(IApplicationDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionKey, CancellationToken ct = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, ct);
        return permissions.Contains(permissionKey);
    }

    public async Task<IReadOnlySet<string>> GetUserPermissionsAsync(Guid userId, CancellationToken ct = default)
    {
        var key = CacheKey(userId);

        if (_cache.TryGetValue<IReadOnlySet<string>>(key, out var cached) && cached is not null)
            return cached;

        var permissions = await ResolvePermissionsFromDb(userId, ct);

        _cache.Set(key, permissions, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration,
            Size = 1
        });

        return permissions;
    }

    public void InvalidateUserCache(Guid userId)
    {
        _cache.Remove(CacheKey(userId));
    }

    public async Task InvalidateRoleCacheAsync(Guid roleId, CancellationToken ct = default)
    {
        // Find all users in this role and invalidate their caches
        var userIds = await _db.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToListAsync(ct);

        foreach (var userId in userIds)
        {
            _cache.Remove(CacheKey(userId));
        }
    }

    private async Task<IReadOnlySet<string>> ResolvePermissionsFromDb(Guid userId, CancellationToken ct)
    {
        // 1. Get role-based permissions (via user's roles)
        var rolePermissions = await (
            from ur in _db.UserRoles.Where(ur => ur.UserId == userId)
            join rp in _db.RolePermissions on ur.RoleId equals rp.RoleId
            join p in _db.Permissions on rp.PermissionId equals p.Id
            select new { p.Resource, p.Action }
        ).ToListAsync(ct);

        // 2. Get direct user permissions
        var directPermissions = await _db.UserPermissions
            .Where(up => up.UserId == userId)
            .Select(up => new
            {
                up.Permission.Resource,
                up.Permission.Action,
                up.IsGranted
            })
            .ToListAsync(ct);

        // 3. Build effective permission set: start with role permissions
        var effective = new HashSet<string>(
            rolePermissions.Select(rp => $"{rp.Resource}.{rp.Action.ToString().ToLowerInvariant()}"));

        // 4. Apply direct overrides
        foreach (var dp in directPermissions)
        {
            var key = $"{dp.Resource}.{dp.Action.ToString().ToLowerInvariant()}";
            if (dp.IsGranted)
                effective.Add(key);
            else
                effective.Remove(key); // Explicit deny
        }

        return effective;
    }
}
