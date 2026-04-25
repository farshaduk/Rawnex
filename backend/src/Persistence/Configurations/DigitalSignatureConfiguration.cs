using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class DigitalSignatureConfiguration : IEntityTypeConfiguration<DigitalSignature>
{
    public void Configure(EntityTypeBuilder<DigitalSignature> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SignerName).IsRequired().HasMaxLength(256);
        builder.Property(s => s.SignerRole).IsRequired().HasMaxLength(100);
        builder.Property(s => s.SignatureHash).IsRequired().HasMaxLength(512);
        builder.Property(s => s.IpAddress).HasMaxLength(45);

        builder.HasIndex(s => s.ContractId);

        builder.HasOne(s => s.SignerUser)
            .WithMany()
            .HasForeignKey(s => s.SignerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.SignerCompany)
            .WithMany()
            .HasForeignKey(s => s.SignerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
