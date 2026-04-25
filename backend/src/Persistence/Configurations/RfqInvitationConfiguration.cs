using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class RfqInvitationConfiguration : IEntityTypeConfiguration<RfqInvitation>
{
    public void Configure(EntityTypeBuilder<RfqInvitation> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => new { i.RfqId, i.SellerCompanyId }).IsUnique();

        builder.HasOne(i => i.SellerCompany)
            .WithMany()
            .HasForeignKey(i => i.SellerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
