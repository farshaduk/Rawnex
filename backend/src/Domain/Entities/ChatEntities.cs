using Rawnex.Domain.Common;

namespace Rawnex.Domain.Entities;

public class ChatConversation : BaseAuditableEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
    public Guid? NegotiationId { get; set; }
    public Guid? PurchaseOrderId { get; set; }
    public Guid? DisputeId { get; set; }
    public string? Subject { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastMessageAt { get; set; }

    // Navigation
    public Negotiation? Negotiation { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    public Dispute? Dispute { get; set; }
    public ICollection<ChatParticipant> Participants { get; set; } = new List<ChatParticipant>();
    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

public class ChatParticipant : BaseAuditableEntity
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastReadAt { get; set; }

    // Navigation
    public ChatConversation Conversation { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public Company Company { get; set; } = default!;
}

public class ChatMessage : BaseAuditableEntity
{
    public Guid ConversationId { get; set; }
    public Guid SenderUserId { get; set; }
    public Guid SenderCompanyId { get; set; }
    public string Content { get; set; } = default!;
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation
    public ChatConversation Conversation { get; set; } = default!;
    public ApplicationUser SenderUser { get; set; } = default!;
    public Company SenderCompany { get; set; } = default!;
}
