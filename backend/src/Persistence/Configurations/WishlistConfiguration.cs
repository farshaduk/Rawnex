using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.CreatedBy).HasMaxLength(256);
        builder.Property(w => w.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(w => w.UserId);
        builder.HasIndex(w => w.TenantId);

        builder.HasOne(w => w.User)
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(w => w.Items)
            .WithOne(i => i.Wishlist)
            .HasForeignKey(i => i.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
