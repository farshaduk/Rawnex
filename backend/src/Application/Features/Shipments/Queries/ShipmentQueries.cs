using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Shipments.DTOs;

namespace Rawnex.Application.Features.Shipments.Queries;

public record GetShipmentByIdQuery(Guid ShipmentId) : IRequest<Result<ShipmentDetailDto>>;
public record GetOrderShipmentsQuery(Guid PurchaseOrderId) : IRequest<Result<List<ShipmentDto>>>;
public record GetFreightQuotesQuery(Guid PurchaseOrderId) : IRequest<Result<List<FreightQuoteDto>>>;
