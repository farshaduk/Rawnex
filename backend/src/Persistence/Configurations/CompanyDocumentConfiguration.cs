using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class CompanyDocumentConfiguration : IEntityTypeConfiguration<CompanyDocument>
{
    public void Configure(EntityTypeBuilder<CompanyDocument> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.FileName).IsRequired().HasMaxLength(256);
        builder.Property(d => d.FileUrl).IsRequired().HasMaxLength(1000);
        builder.Property(d => d.MimeType).HasMaxLength(100);
        builder.Property(d => d.VerificationStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.VerificationNotes).HasMaxLength(1000);
        builder.Property(d => d.VerifiedBy).HasMaxLength(256);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(d => d.CompanyId);
    }
}
