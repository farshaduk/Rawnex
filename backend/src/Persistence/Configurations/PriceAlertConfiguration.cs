using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class PriceAlertConfiguration : IEntityTypeConfiguration<PriceAlert>
{
    public void Configure(EntityTypeBuilder<PriceAlert> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.TargetPrice).HasPrecision(18, 4);
        builder.Property(a => a.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.CreatedBy).HasMaxLength(256);
        builder.Property(a => a.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.ProductId);
        builder.HasIndex(a => a.TenantId);

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Product)
            .WithMany()
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
