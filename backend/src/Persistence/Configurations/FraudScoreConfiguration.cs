using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class FraudScoreConfiguration : IEntityTypeConfiguration<FraudScore>
{
    public void Configure(EntityTypeBuilder<FraudScore> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.CheckType).HasConversion<string>().HasMaxLength(30);
        builder.Property(f => f.RiskLevel).HasConversion<string>().HasMaxLength(20);
        builder.Property(f => f.Score).HasPrecision(5, 2);
        builder.Property(f => f.DetailsJson).HasMaxLength(8000);
        builder.Property(f => f.IpAddress).HasMaxLength(45);
        builder.Property(f => f.DeviceFingerprint).HasMaxLength(256);
        builder.Property(f => f.FlagReason).HasMaxLength(1000);
        builder.Property(f => f.ReviewedBy).HasMaxLength(256);
        builder.Property(f => f.CreatedBy).HasMaxLength(256);
        builder.Property(f => f.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(f => f.UserId);
        builder.HasIndex(f => f.CompanyId);
        builder.HasIndex(f => f.RiskLevel);
        builder.HasIndex(f => f.IsFlagged);

        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.Company)
            .WithMany()
            .HasForeignKey(f => f.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
