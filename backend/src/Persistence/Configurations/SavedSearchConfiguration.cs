using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class SavedSearchConfiguration : IEntityTypeConfiguration<SavedSearch>
{
    public void Configure(EntityTypeBuilder<SavedSearch> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.SearchCriteriaJson).IsRequired().HasMaxLength(4000);
        builder.Property(s => s.CreatedBy).HasMaxLength(256);
        builder.Property(s => s.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.TenantId);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
