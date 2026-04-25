using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.WebApi.GraphQL;

public class Query
{
    [UseFiltering]
    [UseSorting]
    public IQueryable<Listing> GetListings([Service] IApplicationDbContext db)
        => db.Listings.AsNoTracking().Where(l => l.Status == ListingStatus.Active);

    [UseFiltering]
    [UseSorting]
    public IQueryable<Product> GetProducts([Service] IApplicationDbContext db)
        => db.Products.AsNoTracking().Where(p => p.Status == ProductStatus.Active);

    [UseFiltering]
    [UseSorting]
    public IQueryable<Auction> GetAuctions([Service] IApplicationDbContext db)
        => db.Auctions.AsNoTracking();

    [UseFiltering]
    [UseSorting]
    public IQueryable<PurchaseOrder> GetOrders([Service] IApplicationDbContext db)
        => db.PurchaseOrders.AsNoTracking();

    [UseFiltering]
    [UseSorting]
    public IQueryable<Shipment> GetShipments([Service] IApplicationDbContext db)
        => db.Shipments.AsNoTracking();

    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> GetCompanies([Service] IApplicationDbContext db)
        => db.Companies.AsNoTracking();

    [UseFiltering]
    [UseSorting]
    public IQueryable<Rfq> GetRfqs([Service] IApplicationDbContext db)
        => db.Rfqs.AsNoTracking();

    [UseFiltering]
    [UseSorting]
    public IQueryable<Contract> GetContracts([Service] IApplicationDbContext db)
        => db.Contracts.AsNoTracking();

    public async Task<Listing?> GetListingById([Service] IApplicationDbContext db, Guid id)
        => await db.Listings.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);

    public async Task<PurchaseOrder?> GetOrderById([Service] IApplicationDbContext db, Guid id)
        => await db.PurchaseOrders.AsNoTracking()
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<Company?> GetCompanyById([Service] IApplicationDbContext db, Guid id)
        => await db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
}
