namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// Checks whether a user has a specific permission.
/// Implementation uses caching for performance — changes take effect immediately.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if user has the given permission key (e.g. "products.read").
    /// Resolves from: user direct permissions (override) → role permissions.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionKey, CancellationToken ct = default);

    /// <summary>
    /// Get all effective permission keys for a user.
    /// </summary>
    Task<IReadOnlySet<string>> GetUserPermissionsAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Invalidate cached permissions for a user (call after any permission change).
    /// </summary>
    void InvalidateUserCache(Guid userId);

    /// <summary>
    /// Invalidate cached permissions for all users in a role (call after role permission change).
    /// </summary>
    Task InvalidateRoleCacheAsync(Guid roleId, CancellationToken ct = default);
}
