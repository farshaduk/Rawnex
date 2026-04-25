using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class QualityReportConfiguration : IEntityTypeConfiguration<QualityReport>
{
    public void Configure(EntityTypeBuilder<QualityReport> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title).IsRequired().HasMaxLength(300);
        builder.Property(r => r.Summary).HasMaxLength(2000);
        builder.Property(r => r.DetailedFindingsJson).HasMaxLength(8000);
        builder.Property(r => r.FileUrl).HasMaxLength(1000);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(r => r.QualityInspectionId);

        builder.HasMany(r => r.LabTestResults)
            .WithOne(l => l.QualityReport)
            .HasForeignKey(l => l.QualityReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
