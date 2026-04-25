using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class RfqInvitation : BaseEntity
{
    public Guid RfqId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;
    public bool HasResponded { get; set; }

    // Navigation
    public Rfq Rfq { get; set; } = default!;
    public Company SellerCompany { get; set; } = default!;
}
