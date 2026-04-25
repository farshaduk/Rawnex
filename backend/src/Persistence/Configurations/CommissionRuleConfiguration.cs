using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class CommissionRuleConfiguration : IEntityTypeConfiguration<CommissionRule>
{
    public void Configure(EntityTypeBuilder<CommissionRule> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Value).HasPrecision(18, 4);
        builder.Property(c => c.MinTransactionAmount).HasPrecision(18, 4);
        builder.Property(c => c.MaxTransactionAmount).HasPrecision(18, 4);
        builder.Property(c => c.Currency).HasConversion<string>().HasMaxLength(10);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
