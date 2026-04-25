using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class DisputeConfiguration : IEntityTypeConfiguration<Dispute>
{
    public void Configure(EntityTypeBuilder<Dispute> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DisputeNumber).IsRequired().HasMaxLength(50);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(d => d.Reason).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.Description).IsRequired().HasMaxLength(4000);
        builder.Property(d => d.ClaimedAmount).HasPrecision(18, 4);
        builder.Property(d => d.ClaimedCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(d => d.Resolution).HasConversion<string>().HasMaxLength(30);
        builder.Property(d => d.ResolutionNotes).HasMaxLength(4000);
        builder.Property(d => d.ResolvedAmount).HasPrecision(18, 4);
        builder.Property(d => d.ResolvedBy).HasMaxLength(256);
        builder.Property(d => d.CreatedBy).HasMaxLength(256);
        builder.Property(d => d.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(d => d.TenantId);
        builder.HasIndex(d => d.DisputeNumber).IsUnique();
        builder.HasIndex(d => d.PurchaseOrderId);
        builder.HasIndex(d => d.Status);

        builder.Ignore(d => d.DomainEvents);

        builder.HasOne(d => d.PurchaseOrder)
            .WithMany()
            .HasForeignKey(d => d.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.FiledByCompany)
            .WithMany()
            .HasForeignKey(d => d.FiledByCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.FiledByUser)
            .WithMany()
            .HasForeignKey(d => d.FiledByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.AgainstCompany)
            .WithMany()
            .HasForeignKey(d => d.AgainstCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(d => d.Evidence)
            .WithOne(e => e.Dispute)
            .HasForeignKey(e => e.DisputeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
