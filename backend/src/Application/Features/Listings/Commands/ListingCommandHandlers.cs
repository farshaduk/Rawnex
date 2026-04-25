using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Listings.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Listings.Commands;

public class CreateListingCommandHandler : IRequestHandler<CreateListingCommand, Result<ListingDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateListingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ListingDto>> Handle(CreateListingCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.CompanyId, ct);
        if (company is null) throw new NotFoundException(nameof(Company), request.CompanyId);

        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);
        if (product is null) throw new NotFoundException(nameof(Product), request.ProductId);

        var listing = new Listing
        {
            TenantId = company.TenantId,
            CompanyId = request.CompanyId,
            ProductId = request.ProductId,
            Type = request.Type,
            Status = ListingStatus.Draft,
            Title = request.Title,
            Description = request.Description,
            Quantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure,
            Price = request.Price,
            PriceCurrency = request.PriceCurrency,
            PriceUnit = request.PriceUnit,
            MinOrderQuantity = request.MinOrderQuantity,
            Incoterm = request.Incoterm,
            DeliveryLocation = request.DeliveryLocation,
            LeadTimeDays = request.LeadTimeDays,
            ExpiresAt = request.ExpiresAt,
            DeliveryStartDate = request.DeliveryStartDate,
            DeliveryEndDate = request.DeliveryEndDate,
        };

        _db.Listings.Add(listing);
        await _db.SaveChangesAsync(ct);

        return Result<ListingDto>.Success(new ListingDto(
            listing.Id, listing.CompanyId, company.LegalName, listing.ProductId, product.Name,
            listing.Type, listing.Status, listing.Title, listing.Quantity, listing.UnitOfMeasure,
            listing.Price, listing.PriceCurrency, listing.Incoterm, listing.DeliveryLocation,
            listing.ExpiresAt, listing.CreatedAt));
    }
}

public class UpdateListingCommandHandler : IRequestHandler<UpdateListingCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateListingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(UpdateListingCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId && !l.IsDeleted, ct);
        if (listing is null) throw new NotFoundException(nameof(Listing), request.ListingId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == listing.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        if (request.Title is not null) listing.Title = request.Title;
        if (request.Description is not null) listing.Description = request.Description;
        if (request.Quantity.HasValue) listing.Quantity = request.Quantity.Value;
        if (request.Price.HasValue) listing.Price = request.Price.Value;
        if (request.PriceCurrency.HasValue) listing.PriceCurrency = request.PriceCurrency.Value;
        if (request.MinOrderQuantity.HasValue) listing.MinOrderQuantity = request.MinOrderQuantity;
        if (request.DeliveryLocation is not null) listing.DeliveryLocation = request.DeliveryLocation;
        if (request.LeadTimeDays.HasValue) listing.LeadTimeDays = request.LeadTimeDays;
        if (request.ExpiresAt.HasValue) listing.ExpiresAt = request.ExpiresAt;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ChangeListingStatusCommandHandler : IRequestHandler<ChangeListingStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ChangeListingStatusCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ChangeListingStatusCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId && !l.IsDeleted, ct);
        if (listing is null) throw new NotFoundException(nameof(Listing), request.ListingId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == listing.CompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        listing.Status = request.Status;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeleteListingCommandHandler : IRequestHandler<DeleteListingCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteListingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteListingCommand request, CancellationToken ct)
    {
        var listing = await _db.Listings.FirstOrDefaultAsync(l => l.Id == request.ListingId && !l.IsDeleted, ct);
        if (listing is null) throw new NotFoundException(nameof(Listing), request.ListingId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == listing.CompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only company admins can delete listings.");

        listing.IsDeleted = true;
        listing.DeletedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
