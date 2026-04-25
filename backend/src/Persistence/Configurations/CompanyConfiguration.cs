using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.LegalName).IsRequired().HasMaxLength(300);
        builder.Property(c => c.TradeName).HasMaxLength(300);
        builder.Property(c => c.RegistrationNumber).IsRequired().HasMaxLength(100);
        builder.Property(c => c.TaxId).HasMaxLength(100);
        builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(c => c.LogoUrl).HasMaxLength(500);
        builder.Property(c => c.Website).HasMaxLength(500);
        builder.Property(c => c.Description).HasMaxLength(2000);
        builder.Property(c => c.AddressLine1).HasMaxLength(300);
        builder.Property(c => c.AddressLine2).HasMaxLength(300);
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.State).HasMaxLength(100);
        builder.Property(c => c.PostalCode).HasMaxLength(20);
        builder.Property(c => c.Country).HasMaxLength(100);
        builder.Property(c => c.Phone).HasMaxLength(50);
        builder.Property(c => c.Email).HasMaxLength(256);
        builder.Property(c => c.CreditLimit).HasPrecision(18, 2);
        builder.Property(c => c.DefaultCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(c => c.BankName).HasMaxLength(200);
        builder.Property(c => c.BankAccountNumber).HasMaxLength(100);
        builder.Property(c => c.BankIban).HasMaxLength(34);
        builder.Property(c => c.BankSwiftCode).HasMaxLength(11);
        builder.Property(c => c.VerificationStatus).HasConversion<string>().HasMaxLength(30);
        builder.Property(c => c.VerifiedBy).HasMaxLength(256);
        builder.Property(c => c.EsgScore).HasPrecision(5, 2);
        builder.Property(c => c.TrustScore).HasPrecision(5, 2);
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.LastModifiedBy).HasMaxLength(256);
        builder.Property(c => c.DeletedBy).HasMaxLength(256);

        builder.HasIndex(c => new { c.TenantId, c.RegistrationNumber }).IsUnique();
        builder.HasIndex(c => c.TenantId);
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.ParentCompany)
            .WithMany(c => c.Subsidiaries)
            .HasForeignKey(c => c.ParentCompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Departments)
            .WithOne(d => d.Company)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Documents)
            .WithOne(d => d.Company)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.UboRecords)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Members)
            .WithOne(m => m.Company)
            .HasForeignKey(m => m.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Products)
            .WithOne(p => p.Company)
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(c => c.DomainEvents);
    }
}
