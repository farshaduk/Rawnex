using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ProductCertificationConfiguration : IEntityTypeConfiguration<ProductCertification>
{
    public void Configure(EntityTypeBuilder<ProductCertification> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificationType).IsRequired().HasMaxLength(100);
        builder.Property(c => c.CertificationBody).HasMaxLength(200);
        builder.Property(c => c.CertificateNumber).HasMaxLength(100);
        builder.Property(c => c.FileUrl).HasMaxLength(1000);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(c => c.ProductId);
    }
}
