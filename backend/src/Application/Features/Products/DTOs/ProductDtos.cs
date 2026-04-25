using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Products.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? NameFa,
    string Slug,
    string? Sku,
    Guid CategoryId,
    string? CategoryName,
    ProductStatus Status,
    string? Origin,
    string? PurityGrade,
    decimal? BasePrice,
    Currency? PriceCurrency,
    string? MainImageUrl,
    decimal SustainabilityScore,
    DateTime CreatedAt
);

public record ProductDetailDto(
    Guid Id,
    Guid TenantId,
    Guid CompanyId,
    string CompanyName,
    string Name,
    string? NameFa,
    string Slug,
    string? Description,
    string? DescriptionFa,
    string? Sku,
    string? CasNumber,
    Guid CategoryId,
    string? CategoryName,
    ProductStatus Status,
    string? Origin,
    string? PurityGrade,
    PackagingType? Packaging,
    string? UnitOfMeasure,
    decimal? MinOrderQuantity,
    decimal? MaxOrderQuantity,
    decimal? BasePrice,
    Currency? PriceCurrency,
    string? PriceUnit,
    string? MainImageUrl,
    string? ImagesJson,
    string? CoaFileUrl,
    string? SdsFileUrl,
    string? MsdsFileUrl,
    decimal SustainabilityScore,
    int Version,
    DateTime CreatedAt,
    IList<ProductAttributeDto> Attributes,
    IList<ProductVariantDto> Variants,
    IList<ProductCertificationDto> Certifications
);

public record ProductAttributeDto(Guid Id, string Key, string Value);

public record ProductVariantDto(
    Guid Id,
    string Name,
    string? Sku,
    string? Origin,
    string? PurityGrade,
    decimal? Price,
    Currency? PriceCurrency,
    decimal? AvailableQuantity,
    string? UnitOfMeasure
);

public record ProductCertificationDto(
    Guid Id,
    string CertificationType,
    string? CertificationBody,
    string? CertificateNumber,
    string? FileUrl,
    DateTime? IssuedAt,
    DateTime? ExpiresAt,
    bool IsVerified
);

public record ProductCategoryDto(
    Guid Id,
    string Name,
    string? NameFa,
    string Slug,
    Guid? ParentCategoryId,
    int SortOrder,
    bool IsActive
);
