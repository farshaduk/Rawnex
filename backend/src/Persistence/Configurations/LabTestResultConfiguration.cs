using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class LabTestResultConfiguration : IEntityTypeConfiguration<LabTestResult>
{
    public void Configure(EntityTypeBuilder<LabTestResult> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.TestName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.TestMethod).HasMaxLength(200);
        builder.Property(l => l.Parameter).HasMaxLength(200);
        builder.Property(l => l.ExpectedValue).HasMaxLength(200);
        builder.Property(l => l.ActualValue).HasMaxLength(200);
        builder.Property(l => l.Unit).HasMaxLength(50);
        builder.Property(l => l.LabName).HasMaxLength(200);
        builder.Property(l => l.CertificateUrl).HasMaxLength(1000);
        builder.Property(l => l.CreatedBy).HasMaxLength(256);
        builder.Property(l => l.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(l => l.QualityReportId);
    }
}
