using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Shipments.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Shipments.Queries;

public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, Result<ShipmentDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetShipmentByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ShipmentDetailDto>> Handle(GetShipmentByIdQuery request, CancellationToken ct)
    {
        var s = await _db.Shipments.AsNoTracking()
            .Include(x => x.SellerCompany)
            .Include(x => x.BuyerCompany)
            .Include(x => x.TrackingEvents.OrderByDescending(t => t.Timestamp))
            .Include(x => x.Batches)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId, ct);

        if (s is null) throw new NotFoundException(nameof(Shipment), request.ShipmentId);

        return Result<ShipmentDetailDto>.Success(new ShipmentDetailDto(
            s.Id, s.TenantId, s.ShipmentNumber, s.PurchaseOrderId,
            s.SellerCompanyId, s.SellerCompany.LegalName,
            s.BuyerCompanyId, s.BuyerCompany.LegalName,
            s.Status, s.TransportMode, s.Incoterm,
            s.CarrierName, s.CarrierTrackingNumber, s.BillOfLadingNumber, s.BillOfLadingUrl,
            s.ContainerNumber, s.SealNumber,
            s.OriginAddress, s.OriginCity, s.OriginCountry,
            s.DestinationAddress, s.DestinationCity, s.DestinationCountry,
            s.GrossWeightKg, s.NetWeightKg, s.NumberOfPackages,
            s.EstimatedDepartureDate, s.ActualDepartureDate,
            s.EstimatedArrivalDate, s.ActualArrivalDate, s.DeliveredAt,
            s.ShippingCost, s.InsuranceCost, s.CostCurrency, s.CarbonFootprintKg,
            s.CreatedAt,
            s.TrackingEvents.Select(t => new ShipmentTrackingDto(t.Id, t.Status, t.Location, t.Description, t.Timestamp)).ToList(),
            s.Batches.Select(b => new BatchDto(b.Id, b.ProductId, b.BatchNumber, b.LotNumber, b.Quantity, b.UnitOfMeasure, b.ManufacturedDate, b.ExpiryDate, b.Origin, b.QualityGrade)).ToList()));
    }
}

public class GetOrderShipmentsQueryHandler : IRequestHandler<GetOrderShipmentsQuery, Result<List<ShipmentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetOrderShipmentsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<ShipmentDto>>> Handle(GetOrderShipmentsQuery request, CancellationToken ct)
    {
        var list = await _db.Shipments.AsNoTracking()
            .Include(s => s.SellerCompany)
            .Include(s => s.BuyerCompany)
            .Where(s => s.PurchaseOrderId == request.PurchaseOrderId)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new ShipmentDto(
                s.Id, s.ShipmentNumber, s.PurchaseOrderId,
                s.SellerCompanyId, s.SellerCompany.LegalName,
                s.BuyerCompanyId, s.BuyerCompany.LegalName,
                s.Status, s.TransportMode, s.CarrierName,
                s.OriginCity, s.DestinationCity,
                s.EstimatedArrivalDate, s.CreatedAt))
            .ToListAsync(ct);

        return Result<List<ShipmentDto>>.Success(list);
    }
}

public class GetFreightQuotesQueryHandler : IRequestHandler<GetFreightQuotesQuery, Result<List<FreightQuoteDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetFreightQuotesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<FreightQuoteDto>>> Handle(GetFreightQuotesQuery request, CancellationToken ct)
    {
        var list = await _db.FreightQuotes.AsNoTracking()
            .Where(q => q.PurchaseOrderId == request.PurchaseOrderId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new FreightQuoteDto(
                q.Id, q.CarrierName, q.TransportMode, q.OriginCity, q.DestinationCity,
                q.QuotedPrice, q.Currency, q.EstimatedTransitDays, q.ValidUntil, q.IsSelected))
            .ToListAsync(ct);

        return Result<List<FreightQuoteDto>>.Success(list);
    }
}
