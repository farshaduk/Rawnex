using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class NegotiationMessage : BaseEntity
{
    public Guid NegotiationId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid SenderCompanyId { get; set; }
    public string Content { get; set; } = default!;
    public string? AttachmentsJson { get; set; }
    public bool IsCounterOffer { get; set; }
    public string? CounterOfferJson { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }

    // Navigation
    public Negotiation Negotiation { get; set; } = default!;
    public ApplicationUser SenderUser { get; set; } = default!;
    public Company SenderCompany { get; set; } = default!;
}
