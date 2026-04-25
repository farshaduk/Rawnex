using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Products.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateProductCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);

        var slug = request.Name.ToLowerInvariant().Replace(' ', '-');
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");
        var exists = await _db.Products.AnyAsync(p => p.Slug == slug && !p.IsDeleted, ct);
        if (exists) slug += $"-{Guid.NewGuid().ToString()[..8]}";

        var product = new Product
        {
            TenantId = company.TenantId,
            CompanyId = request.CompanyId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            NameFa = request.NameFa,
            Slug = slug,
            Description = request.Description,
            DescriptionFa = request.DescriptionFa,
            Sku = request.Sku,
            CasNumber = request.CasNumber,
            Origin = request.Origin,
            PurityGrade = request.PurityGrade,
            Packaging = request.Packaging,
            UnitOfMeasure = request.UnitOfMeasure,
            MinOrderQuantity = request.MinOrderQuantity,
            MaxOrderQuantity = request.MaxOrderQuantity,
            BasePrice = request.BasePrice,
            PriceCurrency = request.PriceCurrency ?? Currency.USD,
            PriceUnit = request.PriceUnit,
            MainImageUrl = request.MainImageUrl,
            Status = ProductStatus.Draft,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        var category = await _db.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        return Result<ProductDto>.Success(new ProductDto(
            product.Id, product.Name, product.NameFa, product.Slug, product.Sku,
            product.CategoryId, category?.Name, product.Status, product.Origin,
            product.PurityGrade, product.BasePrice, product.PriceCurrency,
            product.MainImageUrl, product.SustainabilityScore ?? 0, product.CreatedAt));
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateProductCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == product.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        if (request.Name is not null) product.Name = request.Name;
        if (request.NameFa is not null) product.NameFa = request.NameFa;
        if (request.Description is not null) product.Description = request.Description;
        if (request.DescriptionFa is not null) product.DescriptionFa = request.DescriptionFa;
        if (request.Origin is not null) product.Origin = request.Origin;
        if (request.PurityGrade is not null) product.PurityGrade = request.PurityGrade;
        if (request.Packaging.HasValue) product.Packaging = request.Packaging;
        if (request.UnitOfMeasure is not null) product.UnitOfMeasure = request.UnitOfMeasure;
        if (request.MinOrderQuantity.HasValue) product.MinOrderQuantity = request.MinOrderQuantity;
        if (request.MaxOrderQuantity.HasValue) product.MaxOrderQuantity = request.MaxOrderQuantity;
        if (request.BasePrice.HasValue) product.BasePrice = request.BasePrice;
        if (request.PriceCurrency.HasValue) product.PriceCurrency = request.PriceCurrency.Value;
        if (request.PriceUnit is not null) product.PriceUnit = request.PriceUnit;
        if (request.MainImageUrl is not null) product.MainImageUrl = request.MainImageUrl;

        product.Version++;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ChangeProductStatusCommandHandler : IRequestHandler<ChangeProductStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ChangeProductStatusCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangeProductStatusCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == product.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        product.Status = request.Status;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteProductCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == product.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only company admins can delete products.");

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CreateProductCategoryCommandHandler : IRequestHandler<CreateProductCategoryCommand, Result<ProductCategoryDto>>
{
    private readonly IApplicationDbContext _db;

    public CreateProductCategoryCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ProductCategoryDto>> Handle(CreateProductCategoryCommand request, CancellationToken ct)
    {
        var slug = request.Name.ToLowerInvariant().Replace(' ', '-');
        slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

        var category = new ProductCategory
        {
            Name = request.Name,
            NameFa = request.NameFa,
            Slug = slug,
            ParentCategoryId = request.ParentCategoryId,
            SortOrder = request.SortOrder,
            IsActive = true,
        };

        _db.ProductCategories.Add(category);
        await _db.SaveChangesAsync(ct);

        return Result<ProductCategoryDto>.Success(new ProductCategoryDto(
            category.Id, category.Name, category.NameFa, category.Slug,
            category.ParentCategoryId, category.SortOrder, category.IsActive));
    }
}

public class AddProductVariantCommandHandler : IRequestHandler<AddProductVariantCommand, Result<ProductVariantDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddProductVariantCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductVariantDto>> Handle(AddProductVariantCommand request, CancellationToken ct)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == product.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            Name = request.Name,
            Sku = request.Sku,
            Origin = request.Origin,
            PurityGrade = request.PurityGrade,
            Price = request.Price,
            PriceCurrency = request.PriceCurrency ?? Currency.USD,
            AvailableQuantity = request.AvailableQuantity,
            UnitOfMeasure = request.UnitOfMeasure,
        };

        _db.ProductVariants.Add(variant);
        await _db.SaveChangesAsync(ct);

        return Result<ProductVariantDto>.Success(new ProductVariantDto(
            variant.Id, variant.Name, variant.Sku, variant.Origin, variant.PurityGrade,
            variant.Price, variant.PriceCurrency, variant.AvailableQuantity, variant.UnitOfMeasure));
    }
}
