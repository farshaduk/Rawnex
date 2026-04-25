using Microsoft.AspNetCore.Identity;

namespace Rawnex.Domain.Entities;

/// <summary>
/// Many-to-many join entity for Users ↔ Roles.
/// Allows navigation and additional metadata on role assignment.
/// </summary>
public class ApplicationUserRole : IdentityUserRole<Guid>
{
    public ApplicationUser User { get; set; } = null!;
    public ApplicationRole Role { get; set; } = null!;

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public string? AssignedBy { get; set; }
}
