using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class KycVerificationConfiguration : IEntityTypeConfiguration<KycVerification>
{
    public void Configure(EntityTypeBuilder<KycVerification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(200);
        builder.Property(x => x.NationalId).HasMaxLength(50);
        builder.Property(x => x.PassportNumber).HasMaxLength(50);
        builder.Property(x => x.Nationality).HasMaxLength(100);
        builder.Property(x => x.AddressLine1).HasMaxLength(500);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Country).HasMaxLength(100);
        builder.Property(x => x.IdDocumentUrl).HasMaxLength(1000);
        builder.Property(x => x.SelfieUrl).HasMaxLength(1000);
        builder.Property(x => x.ProofOfAddressUrl).HasMaxLength(1000);
        builder.Property(x => x.RejectionReason).HasMaxLength(2000);
        builder.Property(x => x.ReviewedBy).HasMaxLength(200);

        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
    }
}

public class KybVerificationConfiguration : IEntityTypeConfiguration<KybVerification>
{
    public void Configure(EntityTypeBuilder<KybVerification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CompanyRegistrationDocUrl).HasMaxLength(1000);
        builder.Property(x => x.TaxCertificateUrl).HasMaxLength(1000);
        builder.Property(x => x.FinancialStatementUrl).HasMaxLength(1000);
        builder.Property(x => x.BankStatementUrl).HasMaxLength(1000);
        builder.Property(x => x.ProductionLicenseUrl).HasMaxLength(1000);
        builder.Property(x => x.ExportLicenseUrl).HasMaxLength(1000);
        builder.Property(x => x.FactoryPhotoUrl).HasMaxLength(1000);
        builder.Property(x => x.RejectionReason).HasMaxLength(2000);
        builder.Property(x => x.ReviewedBy).HasMaxLength(200);

        builder.HasOne(x => x.Company).WithMany().HasForeignKey(x => x.CompanyId);
        builder.HasIndex(x => x.CompanyId);
        builder.HasIndex(x => x.Status);
    }
}
