using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.NameFa).HasMaxLength(200);
        builder.Property(c => c.Slug).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasMaxLength(1000);
        builder.Property(c => c.IconUrl).HasMaxLength(500);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(c => c.Slug).IsUnique();

        builder.Ignore(c => c.DomainEvents);

        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
