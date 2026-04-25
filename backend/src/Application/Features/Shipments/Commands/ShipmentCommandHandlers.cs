using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Shipments.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Shipments.Commands;

public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Result<ShipmentDto>>
{
    private readonly IApplicationDbContext _db;

    public CreateShipmentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ShipmentDto>> Handle(CreateShipmentCommand request, CancellationToken ct)
    {
        var seller = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);
        if (seller is null) throw new NotFoundException(nameof(Company), request.SellerCompanyId);

        var buyer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);

        var shipmentNumber = $"SHP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var shipment = new Shipment
        {
            TenantId = seller.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            SellerCompanyId = request.SellerCompanyId,
            BuyerCompanyId = request.BuyerCompanyId,
            ShipmentNumber = shipmentNumber,
            Status = ShipmentStatus.Pending,
            TransportMode = request.TransportMode,
            Incoterm = request.Incoterm,
            CarrierName = request.CarrierName,
            CarrierTrackingNumber = request.CarrierTrackingNumber,
            ContainerNumber = request.ContainerNumber,
            OriginAddress = request.OriginAddress,
            OriginCity = request.OriginCity,
            OriginCountry = request.OriginCountry,
            DestinationAddress = request.DestinationAddress,
            DestinationCity = request.DestinationCity,
            DestinationCountry = request.DestinationCountry,
            GrossWeightKg = request.GrossWeightKg,
            NumberOfPackages = request.NumberOfPackages,
            EstimatedDepartureDate = request.EstimatedDepartureDate,
            EstimatedArrivalDate = request.EstimatedArrivalDate,
            ShippingCost = request.ShippingCost,
            CostCurrency = request.CostCurrency,
        };

        _db.Shipments.Add(shipment);
        await _db.SaveChangesAsync(ct);

        return Result<ShipmentDto>.Success(new ShipmentDto(
            shipment.Id, shipment.ShipmentNumber, shipment.PurchaseOrderId,
            shipment.SellerCompanyId, seller.LegalName, shipment.BuyerCompanyId, buyer?.LegalName,
            shipment.Status, shipment.TransportMode, shipment.CarrierName,
            shipment.OriginCity, shipment.DestinationCity,
            shipment.EstimatedArrivalDate, shipment.CreatedAt));
    }
}

public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateShipmentStatusCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateShipmentStatusCommand request, CancellationToken ct)
    {
        var shipment = await _db.Shipments.FirstOrDefaultAsync(s => s.Id == request.ShipmentId, ct);
        if (shipment is null) throw new NotFoundException(nameof(Shipment), request.ShipmentId);

        var oldStatus = shipment.Status;
        shipment.Status = request.Status;

        if (request.Status == ShipmentStatus.InTransit && !shipment.ActualDepartureDate.HasValue)
            shipment.ActualDepartureDate = DateTime.UtcNow;
        if (request.Status == ShipmentStatus.Delivered)
            shipment.DeliveredAt = DateTime.UtcNow;

        _db.ShipmentTrackings.Add(new ShipmentTracking
        {
            ShipmentId = request.ShipmentId,
            Status = request.Status,
            Location = request.Location,
            Description = request.Description,
        });

        shipment.AddDomainEvent(new ShipmentStatusChangedEvent(shipment.Id, oldStatus, request.Status));
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class AddBatchToShipmentCommandHandler : IRequestHandler<AddBatchToShipmentCommand, Result<BatchDto>>
{
    private readonly IApplicationDbContext _db;

    public AddBatchToShipmentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<BatchDto>> Handle(AddBatchToShipmentCommand request, CancellationToken ct)
    {
        var shipment = await _db.Shipments.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.ShipmentId, ct);
        if (shipment is null) throw new NotFoundException(nameof(Shipment), request.ShipmentId);

        var batch = new Batch
        {
            TenantId = shipment.TenantId,
            ShipmentId = request.ShipmentId,
            ProductId = request.ProductId,
            BatchNumber = request.BatchNumber,
            LotNumber = request.LotNumber,
            Quantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure,
            ManufacturedDate = request.ManufacturedDate,
            ExpiryDate = request.ExpiryDate,
            Origin = request.Origin,
            QualityGrade = request.QualityGrade,
            CoaFileUrl = request.CoaFileUrl,
        };

        _db.Batches.Add(batch);
        await _db.SaveChangesAsync(ct);

        return Result<BatchDto>.Success(new BatchDto(
            batch.Id, batch.ProductId, batch.BatchNumber, batch.LotNumber,
            batch.Quantity, batch.UnitOfMeasure, batch.ManufacturedDate,
            batch.ExpiryDate, batch.Origin, batch.QualityGrade));
    }
}

public class RequestFreightQuoteCommandHandler : IRequestHandler<RequestFreightQuoteCommand, Result<FreightQuoteDto>>
{
    private readonly IApplicationDbContext _db;

    public RequestFreightQuoteCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<FreightQuoteDto>> Handle(RequestFreightQuoteCommand request, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.PurchaseOrderId, ct);
        if (order is null) throw new NotFoundException(nameof(PurchaseOrder), request.PurchaseOrderId);

        var quote = new FreightQuote
        {
            TenantId = order.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            CarrierName = request.CarrierName,
            TransportMode = request.TransportMode,
            OriginCity = request.OriginCity,
            OriginCountry = request.OriginCountry,
            DestinationCity = request.DestinationCity,
            DestinationCountry = request.DestinationCountry,
            QuotedPrice = request.QuotedPrice,
            Currency = request.Currency,
            EstimatedTransitDays = request.EstimatedTransitDays,
            ValidUntil = request.ValidUntil,
        };

        _db.FreightQuotes.Add(quote);
        await _db.SaveChangesAsync(ct);

        return Result<FreightQuoteDto>.Success(new FreightQuoteDto(
            quote.Id, quote.CarrierName, quote.TransportMode, quote.OriginCity, quote.DestinationCity,
            quote.QuotedPrice, quote.Currency, quote.EstimatedTransitDays, quote.ValidUntil, false));
    }
}

public class SelectFreightQuoteCommandHandler : IRequestHandler<SelectFreightQuoteCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public SelectFreightQuoteCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(SelectFreightQuoteCommand request, CancellationToken ct)
    {
        var quote = await _db.FreightQuotes.FirstOrDefaultAsync(q => q.Id == request.FreightQuoteId, ct);
        if (quote is null) throw new NotFoundException(nameof(FreightQuote), request.FreightQuoteId);

        // Deselect other quotes for same order
        var otherQuotes = await _db.FreightQuotes
            .Where(q => q.PurchaseOrderId == quote.PurchaseOrderId && q.Id != quote.Id && q.IsSelected)
            .ToListAsync(ct);
        foreach (var q in otherQuotes) q.IsSelected = false;

        quote.IsSelected = true;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
