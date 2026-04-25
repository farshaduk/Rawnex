using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

/// <summary>
/// Immutable audit log entry for security-sensitive actions.
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public AuditAction Action { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsSuccess { get; set; }
}
