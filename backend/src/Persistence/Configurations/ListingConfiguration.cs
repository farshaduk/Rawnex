using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rawnex.Domain.Entities;

namespace Rawnex.Persistence.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title).IsRequired().HasMaxLength(300);
        builder.Property(l => l.Description).HasMaxLength(4000);
        builder.Property(l => l.UnitOfMeasure).IsRequired().HasMaxLength(20);
        builder.Property(l => l.Type).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.Quantity).HasPrecision(18, 4);
        builder.Property(l => l.Price).HasPrecision(18, 4);
        builder.Property(l => l.PriceCurrency).HasConversion<string>().HasMaxLength(10);
        builder.Property(l => l.PriceUnit).HasMaxLength(50);
        builder.Property(l => l.MinOrderQuantity).HasPrecision(18, 4);
        builder.Property(l => l.Incoterm).HasConversion<string>().HasMaxLength(10);
        builder.Property(l => l.DeliveryLocation).HasMaxLength(500);
        builder.Property(l => l.CreatedBy).HasMaxLength(256);
        builder.Property(l => l.LastModifiedBy).HasMaxLength(256);
        builder.Property(l => l.DeletedBy).HasMaxLength(256);

        builder.HasIndex(l => l.TenantId);
        builder.HasIndex(l => l.CompanyId);
        builder.HasIndex(l => l.ProductId);
        builder.HasIndex(l => l.Status);
        builder.HasQueryFilter(l => !l.IsDeleted);

        builder.Ignore(l => l.DomainEvents);

        builder.HasOne(l => l.Company)
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(l => l.Product)
            .WithMany()
            .HasForeignKey(l => l.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
