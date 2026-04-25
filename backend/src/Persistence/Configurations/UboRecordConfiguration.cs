using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class UboRecordConfiguration : IEntityTypeConfiguration<UboRecord>
{
    public void Configure(EntityTypeBuilder<UboRecord> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(200);
        builder.Property(u => u.NationalId).HasMaxLength(50);
        builder.Property(u => u.PassportNumber).HasMaxLength(50);
        builder.Property(u => u.Nationality).HasMaxLength(100);
        builder.Property(u => u.OwnershipPercentage).HasPrecision(5, 2);
        builder.Property(u => u.AddressLine1).HasMaxLength(300);
        builder.Property(u => u.City).HasMaxLength(100);
        builder.Property(u => u.Country).HasMaxLength(100);
        builder.Property(u => u.CreatedBy).HasMaxLength(256);
        builder.Property(u => u.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(u => u.CompanyId);
    }
}
