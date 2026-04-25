using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(30);
        builder.Property(n => n.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(300);
        builder.Property(n => n.Message).HasMaxLength(2000);
        builder.Property(n => n.ActionUrl).HasMaxLength(1000);
        builder.Property(n => n.DataJson).HasMaxLength(4000);

        builder.HasIndex(n => n.UserId);
        builder.HasIndex(n => n.TenantId);
        builder.HasIndex(n => new { n.UserId, n.IsRead });

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
