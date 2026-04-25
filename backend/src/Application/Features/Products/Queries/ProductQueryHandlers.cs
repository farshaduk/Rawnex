using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Products.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Products.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetProductByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProductDetailDto>> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var p = await _db.Products.AsNoTracking()
            .Include(x => x.Company)
            .Include(x => x.Category)
            .Include(x => x.Attributes.OrderBy(a => a.SortOrder))
            .Include(x => x.Variants.Where(v => v.IsActive))
            .Include(x => x.Certifications)
            .FirstOrDefaultAsync(x => x.Id == request.ProductId && !x.IsDeleted, ct);

        if (p is null) throw new NotFoundException(nameof(Product), request.ProductId);

        return Result<ProductDetailDto>.Success(new ProductDetailDto(
            p.Id, p.TenantId, p.CompanyId, p.Company.LegalName,
            p.Name, p.NameFa, p.Slug, p.Description, p.DescriptionFa,
            p.Sku, p.CasNumber, p.CategoryId, p.Category.Name,
            p.Status, p.Origin, p.PurityGrade, p.Packaging,
            p.UnitOfMeasure, p.MinOrderQuantity, p.MaxOrderQuantity,
            p.BasePrice, p.PriceCurrency, p.PriceUnit,
            p.MainImageUrl, p.ImagesJson, p.CoaFileUrl, p.SdsFileUrl, p.MsdsFileUrl,
            p.SustainabilityScore ?? 0, p.Version, p.CreatedAt,
            p.Attributes.Select(a => new ProductAttributeDto(a.Id, a.Key, a.Value)).ToList(),
            p.Variants.Select(v => new ProductVariantDto(v.Id, v.Name, v.Sku, v.Origin, v.PurityGrade, v.Price, v.PriceCurrency, v.AvailableQuantity, v.UnitOfMeasure)).ToList(),
            p.Certifications.Select(c => new ProductCertificationDto(c.Id, c.CertificationType, c.CertificationBody, c.CertificateNumber, c.FileUrl, c.IssuedAt, c.ExpiresAt, c.IsVerified)).ToList()));
    }
}

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, Result<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _db;

    public ListProductsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<ProductDto>>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        var query = _db.Products.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => !p.IsDeleted);

        if (request.CompanyId.HasValue)
            query = query.Where(p => p.CompanyId == request.CompanyId.Value);
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm) || (p.NameFa != null && p.NameFa.Contains(request.SearchTerm)));

        var result = await query
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new ProductDto(
                p.Id, p.Name, p.NameFa, p.Slug, p.Sku,
                p.CategoryId, p.Category.Name, p.Status, p.Origin,
                p.PurityGrade, p.BasePrice, p.PriceCurrency,
                p.MainImageUrl, p.SustainabilityScore ?? 0, p.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<ProductDto>>.Success(result);
    }
}

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, Result<List<ProductCategoryDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCategoriesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ProductCategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var query = _db.ProductCategories.AsNoTracking().Where(c => c.IsActive);

        if (request.ParentCategoryId.HasValue)
            query = query.Where(c => c.ParentCategoryId == request.ParentCategoryId.Value);
        else
            query = query.Where(c => c.ParentCategoryId == null);

        var categories = await query.OrderBy(c => c.SortOrder)
            .Select(c => new ProductCategoryDto(c.Id, c.Name, c.NameFa, c.Slug, c.ParentCategoryId, c.SortOrder, c.IsActive))
            .ToListAsync(ct);

        return Result<List<ProductCategoryDto>>.Success(categories);
    }
}
