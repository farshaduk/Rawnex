using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Key).IsRequired().HasMaxLength(100);
        builder.Property(f => f.Description).HasMaxLength(500);
        builder.Property(f => f.RolloutPercentage).HasPrecision(5, 2);
        builder.Property(f => f.TargetTenantsJson).HasMaxLength(4000);
        builder.Property(f => f.TargetRolesJson).HasMaxLength(2000);
        builder.Property(f => f.CreatedBy).HasMaxLength(256);
        builder.Property(f => f.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(f => f.Key).IsUnique();

        builder.Ignore(f => f.IsEnabledForAll);
    }
}
