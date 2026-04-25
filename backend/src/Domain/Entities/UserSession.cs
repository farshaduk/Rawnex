using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

/// <summary>
/// Tracks device/browser sessions for a user.
/// Enables "manage active sessions" functionality and per-device revocation.
/// </summary>
public class UserSession : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public string DeviceInfo { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public bool IsActive => !IsRevoked;
}
