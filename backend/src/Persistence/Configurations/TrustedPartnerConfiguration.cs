using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class TrustedPartnerConfiguration : IEntityTypeConfiguration<TrustedPartner>
{
    public void Configure(EntityTypeBuilder<TrustedPartner> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Notes).HasMaxLength(500);
        builder.Property(tp => tp.CreatedBy).HasMaxLength(256);
        builder.Property(tp => tp.LastModifiedBy).HasMaxLength(256);

        builder.HasIndex(tp => new { tp.CompanyId, tp.PartnerCompanyId }).IsUnique();

        builder.HasOne(tp => tp.PartnerCompany)
            .WithMany()
            .HasForeignKey(tp => tp.PartnerCompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
