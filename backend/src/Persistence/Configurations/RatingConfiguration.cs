using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment).HasMaxLength(2000);
        builder.Property(r => r.ResponseComment).HasMaxLength(2000);
        builder.Property(r => r.CreatedBy).HasMaxLength(256);
        builder.Property(r => r.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(r => r.TenantId);
        builder.HasIndex(r => r.PurchaseOrderId);
        builder.HasIndex(r => r.ReviewedCompanyId);
        builder.HasIndex(r => new { r.PurchaseOrderId, r.ReviewerCompanyId }).IsUnique();

        builder.HasOne(r => r.PurchaseOrder)
            .WithMany()
            .HasForeignKey(r => r.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewerCompany)
            .WithMany()
            .HasForeignKey(r => r.ReviewerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewerUser)
            .WithMany()
            .HasForeignKey(r => r.ReviewerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ReviewedCompany)
            .WithMany()
            .HasForeignKey(r => r.ReviewedCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
