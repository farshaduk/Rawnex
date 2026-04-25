using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
{
    public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Url).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Secret).HasMaxLength(500);
        builder.Property(x => x.LastErrorMessage).HasMaxLength(2000);
        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        builder.HasIndex(x => new { x.CompanyId, x.EventType });
    }
}

public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
{
    public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventType).HasMaxLength(200).IsRequired();
        builder.HasOne(x => x.Subscription).WithMany().HasForeignKey(x => x.SubscriptionId);
        builder.HasIndex(x => x.SubscriptionId);
        builder.HasIndex(x => x.IsSuccess);
    }
}
