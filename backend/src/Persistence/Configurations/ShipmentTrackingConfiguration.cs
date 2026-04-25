using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ShipmentTrackingConfiguration : IEntityTypeConfiguration<ShipmentTracking>
{
    public void Configure(EntityTypeBuilder<ShipmentTracking> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(t => t.Location).HasMaxLength(300);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Source).HasMaxLength(100);

        builder.HasIndex(t => t.ShipmentId);
    }
}
