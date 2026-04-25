using Microsoft.AspNetCore.Identity;

namespace Rawnex.Domain.Entities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    // Predefined system roles
    public const string Admin = nameof(Admin);
    public const string BranchAdmin = nameof(BranchAdmin);
    public const string DepartmentManager = nameof(DepartmentManager);
    public const string User = nameof(User);
}
