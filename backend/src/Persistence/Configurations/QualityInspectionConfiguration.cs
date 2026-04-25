using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class QualityInspectionConfiguration : IEntityTypeConfiguration<QualityInspection>
{
    public void Configure(EntityTypeBuilder<QualityInspection> builder)
    {
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(q => q.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(q => q.Notes).HasMaxLength(2000);
        builder.Property(q => q.PhotosJson).HasMaxLength(4000);
        builder.Property(q => q.AiQualityScore).HasPrecision(5, 2);
        builder.Property(q => q.AiAnalysisJson).HasMaxLength(8000);
        builder.Property(q => q.CreatedBy).HasMaxLength(256);
        builder.Property(q => q.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(q => q.TenantId);
        builder.HasIndex(q => q.PurchaseOrderId);
        builder.HasIndex(q => q.Status);

        builder.Ignore(q => q.DomainEvents);

        builder.HasOne(q => q.PurchaseOrder)
            .WithMany()
            .HasForeignKey(q => q.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Shipment)
            .WithMany()
            .HasForeignKey(q => q.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Batch)
            .WithMany()
            .HasForeignKey(q => q.BatchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.InspectorUser)
            .WithMany()
            .HasForeignKey(q => q.InspectorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.Reports)
            .WithOne(r => r.QualityInspection)
            .HasForeignKey(r => r.QualityInspectionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
