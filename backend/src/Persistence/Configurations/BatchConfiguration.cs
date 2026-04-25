using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BatchNumber).IsRequired().HasMaxLength(100);
        builder.Property(b => b.LotNumber).HasMaxLength(100);
        builder.Property(b => b.Quantity).HasPrecision(18, 4);
        builder.Property(b => b.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(b => b.CoaFileUrl).HasMaxLength(1000);
        builder.Property(b => b.Origin).HasMaxLength(100);
        builder.Property(b => b.QualityGrade).HasMaxLength(50);
        builder.Property(b => b.CreatedBy).HasMaxLength(256);
        builder.Property(b => b.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => b.ShipmentId);
        builder.HasIndex(b => b.BatchNumber);

        builder.HasOne(b => b.Product)
            .WithMany()
            .HasForeignKey(b => b.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
