namespace Rawnex.Domain.Entities;

/// <summary>
/// Many-to-many: Role ↔ Permission.
/// Roles act as containers for permissions.
/// </summary>
public class RolePermission
{
    public Guid RoleId { get; set; }
    public ApplicationRole Role { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public string? GrantedBy { get; set; }
}
