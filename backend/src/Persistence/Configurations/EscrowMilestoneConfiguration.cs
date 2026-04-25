using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class EscrowMilestoneConfiguration : IEntityTypeConfiguration<EscrowMilestone>
{
    public void Configure(EntityTypeBuilder<EscrowMilestone> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
        builder.Property(m => m.ReleasePercentage).HasPrecision(5, 2);
        builder.Property(m => m.ReleaseAmount).HasPrecision(18, 4);
        builder.Property(m => m.CompletedBy).HasMaxLength(256);
        builder.Property(m => m.EvidenceUrl).HasMaxLength(1000);
        builder.Property(m => m.Notes).HasMaxLength(1000);
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(m => m.EscrowAccountId);
    }
}
