using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Products.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Products.Commands;

public record CreateProductCommand(
    Guid CompanyId,
    Guid CategoryId,
    string Name,
    string? NameFa,
    string? Description,
    string? DescriptionFa,
    string? Sku,
    string? CasNumber,
    string? Origin,
    string? PurityGrade,
    PackagingType? Packaging,
    string? UnitOfMeasure,
    decimal? MinOrderQuantity,
    decimal? MaxOrderQuantity,
    decimal? BasePrice,
    Currency? PriceCurrency,
    string? PriceUnit,
    string? MainImageUrl
) : IRequest<Result<ProductDto>>;

public record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? NameFa,
    string? Description,
    string? DescriptionFa,
    string? Origin,
    string? PurityGrade,
    PackagingType? Packaging,
    string? UnitOfMeasure,
    decimal? MinOrderQuantity,
    decimal? MaxOrderQuantity,
    decimal? BasePrice,
    Currency? PriceCurrency,
    string? PriceUnit,
    string? MainImageUrl
) : IRequest<Result>;

public record ChangeProductStatusCommand(
    Guid ProductId,
    ProductStatus Status
) : IRequest<Result>;

public record DeleteProductCommand(Guid ProductId) : IRequest<Result>;

public record CreateProductCategoryCommand(
    string Name,
    string? NameFa,
    Guid? ParentCategoryId,
    int SortOrder = 0
) : IRequest<Result<ProductCategoryDto>>;

public record AddProductVariantCommand(
    Guid ProductId,
    string Name,
    string? Sku,
    string? Origin,
    string? PurityGrade,
    decimal? Price,
    Currency? PriceCurrency,
    decimal? AvailableQuantity,
    string? UnitOfMeasure
) : IRequest<Result<ProductVariantDto>>;
