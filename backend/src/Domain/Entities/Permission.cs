using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

/// <summary>
/// A system permission defining access to a specific resource+action.
/// Format: "{Resource}.{Action}" e.g. "products.read", "orders.manage"
/// </summary>
public class Permission : BaseEntity
{
    public string Resource { get; set; } = string.Empty;
    public PermissionAction Action { get; set; }

    /// <summary>
    /// The full key: "products.read", "users.manage", etc.
    /// </summary>
    public string Key => $"{Resource}.{Action.ToString().ToLowerInvariant()}";

    public string? Description { get; set; }

    /// <summary>
    /// System permissions are auto-seeded and cannot be deleted.
    /// </summary>
    public bool IsSystem { get; set; }

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
