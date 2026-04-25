using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class TenantSettingsConfiguration : IEntityTypeConfiguration<TenantSettings>
{
    public void Configure(EntityTypeBuilder<TenantSettings> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.DefaultCurrency).IsRequired().HasMaxLength(10);
        builder.Property(s => s.DefaultLanguage).IsRequired().HasMaxLength(10);
        builder.Property(s => s.TimeZone).IsRequired().HasMaxLength(50);
        builder.Property(s => s.CustomBrandingJson).HasMaxLength(4000);

        builder.HasIndex(s => s.TenantId).IsUnique();
    }
}
