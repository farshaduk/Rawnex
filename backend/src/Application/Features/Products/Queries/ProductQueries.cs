using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Products.DTOs;

namespace Rawnex.Application.Features.Products.Queries;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDetailDto>>;

public record ListProductsQuery(
    Guid? CompanyId,
    Guid? CategoryId,
    string? SearchTerm,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<ProductDto>>>;

public record GetCategoriesQuery(Guid? ParentCategoryId = null) : IRequest<Result<List<ProductCategoryDto>>>;
