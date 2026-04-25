using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class NotificationPreference : BaseEntity
{
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public bool InApp { get; set; } = true;
    public bool Email { get; set; } = true;
    public bool Sms { get; set; }
    public bool Push { get; set; } = true;

    // Navigation
    public ApplicationUser User { get; set; } = default!;
}
