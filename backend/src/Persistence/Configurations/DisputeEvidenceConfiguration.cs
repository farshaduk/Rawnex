using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class DisputeEvidenceConfiguration : IEntityTypeConfiguration<DisputeEvidence>
{
    public void Configure(EntityTypeBuilder<DisputeEvidence> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.FileUrl).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.MimeType).HasMaxLength(100);
        builder.Property(e => e.CreatedBy).HasMaxLength(256);
        builder.Property(e => e.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(e => e.DisputeId);

        builder.HasOne(e => e.UploadedByUser)
            .WithMany()
            .HasForeignKey(e => e.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
