using Rawnex.Domain.Common;
using Rawnex.Domain.Enums;

namespace Rawnex.Domain.Entities;

public class Product : BaseAuditableEntity, IAggregateRoot, ITenantEntity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? NameFa { get; set; }
    public string Slug { get; set; } = default!;
    public string? Description { get; set; }
    public string? DescriptionFa { get; set; }
    public string? Sku { get; set; }
    public string? CasNumber { get; set; }
    public ProductStatus Status { get; set; }

    // Specifications
    public string? PurityGrade { get; set; }
    public string? Origin { get; set; }
    public PackagingType? Packaging { get; set; }
    public string? UnitOfMeasure { get; set; }
    public decimal? MinOrderQuantity { get; set; }
    public decimal? MaxOrderQuantity { get; set; }

    // Pricing
    public decimal? BasePrice { get; set; }
    public Currency PriceCurrency { get; set; }
    public string? PriceUnit { get; set; }

    // Media
    public string? MainImageUrl { get; set; }
    public string? ImagesJson { get; set; }

    // Documents
    public string? CoaFileUrl { get; set; }
    public string? SdsFileUrl { get; set; }
    public string? MsdsFileUrl { get; set; }

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    // Scores
    public decimal? SustainabilityScore { get; set; }
    public int ViewCount { get; set; }
    public int InquiryCount { get; set; }

    // Version control
    public int Version { get; set; } = 1;

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation
    public Company Company { get; set; } = default!;
    public ProductCategory Category { get; set; } = default!;
    public ICollection<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductCertification> Certifications { get; set; } = new List<ProductCertification>();
}
