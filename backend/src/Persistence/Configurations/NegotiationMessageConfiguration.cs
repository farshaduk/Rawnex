using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class NegotiationMessageConfiguration : IEntityTypeConfiguration<NegotiationMessage>
{
    public void Configure(EntityTypeBuilder<NegotiationMessage> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Content).IsRequired().HasMaxLength(4000);
        builder.Property(m => m.AttachmentsJson).HasMaxLength(4000);
        builder.Property(m => m.CounterOfferJson).HasMaxLength(4000);

        builder.HasIndex(m => m.NegotiationId);
        builder.HasIndex(m => m.SenderUserId);

        builder.HasOne(m => m.SenderUser)
            .WithMany()
            .HasForeignKey(m => m.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.SenderCompany)
            .WithMany()
            .HasForeignKey(m => m.SenderCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
