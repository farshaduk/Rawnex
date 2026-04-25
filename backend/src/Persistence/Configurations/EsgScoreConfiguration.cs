using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class EsgScoreConfiguration : IEntityTypeConfiguration<EsgScore>
{
    public void Configure(EntityTypeBuilder<EsgScore> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnvironmentalScore).HasPrecision(5, 2);
        builder.Property(e => e.SocialScore).HasPrecision(5, 2);
        builder.Property(e => e.GovernanceScore).HasPrecision(5, 2);
        builder.Property(e => e.OverallScore).HasPrecision(5, 2);
        builder.Property(e => e.AssessmentDetailsJson).HasMaxLength(8000);
        builder.Property(e => e.CertificationUrl).HasMaxLength(1000);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(e => e.CompanyId);

        builder.HasOne(e => e.Company)
            .WithMany()
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
