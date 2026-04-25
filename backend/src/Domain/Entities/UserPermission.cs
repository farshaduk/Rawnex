namespace Rawnex.Domain.Entities;

/// <summary>
/// Direct permission grant/denial to a user (overrides role permissions).
/// IsGranted=true means explicitly granted; false means explicitly denied.
/// </summary>
public class UserPermission
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public Guid PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;

    /// <summary>
    /// true = explicitly granted (even if role doesn't have it).
    /// false = explicitly denied (even if role grants it).
    /// </summary>
    public bool IsGranted { get; set; } = true;

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public string? GrantedBy { get; set; }
}
