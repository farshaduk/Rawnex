using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class SanctionCheckConfiguration : IEntityTypeConfiguration<SanctionCheck>
{
    public void Configure(EntityTypeBuilder<SanctionCheck> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CheckedAgainst).IsRequired().HasMaxLength(200);
        builder.Property(s => s.MatchDetailsJson).HasMaxLength(4000);
        builder.Property(s => s.RiskLevel).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.CreatedBy).HasMaxLength(256);
        builder.Property(s => s.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(s => s.CompanyId);

        builder.HasOne(s => s.Company)
            .WithMany()
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
