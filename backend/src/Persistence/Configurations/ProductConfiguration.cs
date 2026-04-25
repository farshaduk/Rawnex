using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(300);
        builder.Property(p => p.NameFa).HasMaxLength(300);
        builder.Property(p => p.Slug).IsRequired().HasMaxLength(300);
        builder.Property(p => p.Description).HasMaxLength(4000);
        builder.Property(p => p.DescriptionFa).HasMaxLength(4000);
        builder.Property(p => p.Sku).HasMaxLength(50);
        builder.Property(p => p.CasNumber).HasMaxLength(20);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.PurityGrade).HasMaxLength(50);
        builder.Property(p => p.Origin).HasMaxLength(100);
        builder.Property(p => p.Packaging).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.UnitOfMeasure).HasMaxLength(20);
        builder.Property(p => p.MinOrderQuantity).HasPrecision(18, 4);
        builder.Property(p => p.MaxOrderQuantity).HasPrecision(18, 4);
        builder.Property(p => p.BasePrice).HasPrecision(18, 4);
        builder.Property(p => p.PriceCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(p => p.PriceUnit).HasMaxLength(50);
        builder.Property(p => p.MainImageUrl).HasMaxLength(1000);
        builder.Property(p => p.ImagesJson).HasMaxLength(4000);
        builder.Property(p => p.CoaFileUrl).HasMaxLength(1000);
        builder.Property(p => p.SdsFileUrl).HasMaxLength(1000);
        builder.Property(p => p.MsdsFileUrl).HasMaxLength(1000);
        builder.Property(p => p.MetaTitle).HasMaxLength(200);
        builder.Property(p => p.MetaDescription).HasMaxLength(500);
        builder.Property(p => p.SustainabilityScore).HasPrecision(5, 2);
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.LastModifiedBy).HasMaxLength(256);
        builder.Property(p => p.DeletedBy).HasMaxLength(256);

        builder.HasIndex(p => new { p.TenantId, p.Slug }).IsUnique();
        builder.HasIndex(p => p.CompanyId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Status);
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Ignore(p => p.DomainEvents);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Attributes)
            .WithOne(a => a.Product)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Variants)
            .WithOne(v => v.Product)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.Certifications)
            .WithOne(c => c.Product)
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
