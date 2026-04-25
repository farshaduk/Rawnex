using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Subject).HasMaxLength(500);
        builder.HasOne(x => x.Negotiation).WithMany().HasForeignKey(x => x.NegotiationId);
        builder.HasOne(x => x.PurchaseOrder).WithMany().HasForeignKey(x => x.PurchaseOrderId);
        builder.HasOne(x => x.Dispute).WithMany().HasForeignKey(x => x.DisputeId);
        builder.HasIndex(x => x.NegotiationId);
    }
}

public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
{
    public void Configure(EntityTypeBuilder<ChatParticipant> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.Conversation).WithMany(c => c.Participants).HasForeignKey(x => x.ConversationId);
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => new { x.ConversationId, x.UserId }).IsUnique();
    }
}

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Content).HasMaxLength(8000).IsRequired();
        builder.Property(x => x.AttachmentUrl).HasMaxLength(1000);
        builder.Property(x => x.AttachmentType).HasMaxLength(100);
        builder.HasOne(x => x.Conversation).WithMany(c => c.Messages).HasForeignKey(x => x.ConversationId);
        builder.HasOne(x => x.SenderUser).WithMany().HasForeignKey(x => x.SenderUserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.SenderCompany).WithMany().HasForeignKey(x => x.SenderCompanyId).OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(x => x.ConversationId);
    }
}
