using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

/// <summary>
/// Stores hashed refresh tokens with rotation and reuse detection support.
/// Each token belongs to a specific user session (device).
/// </summary>
public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    /// <summary>SHA-256 hash of the refresh token. Never store plaintext.</summary>
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>When this token was consumed (rotated). Null if still active.</summary>
    public DateTime? ConsumedAt { get; set; }

    /// <summary>When this token was explicitly revoked (logout / admin action).</summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>Reason for revocation.</summary>
    public string? RevokedReason { get; set; }

    /// <summary>The token that replaced this one after rotation.</summary>
    public Guid? ReplacedByTokenId { get; set; }
    public RefreshToken? ReplacedByToken { get; set; }

    /// <summary>Links to a specific device/session.</summary>
    public Guid? SessionId { get; set; }
    public UserSession? Session { get; set; }

    // Token family for reuse detection
    /// <summary>All tokens in a rotation chain share the same family ID.</summary>
    public Guid TokenFamily { get; set; } = Guid.NewGuid();

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsConsumed => ConsumedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked && !IsConsumed;
}
