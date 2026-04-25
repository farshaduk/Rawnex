using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Quality.DTOs;

namespace Rawnex.Application.Features.Quality.Queries;

public record GetInspectionByIdQuery(Guid InspectionId) : IRequest<Result<QualityInspectionDetailDto>>;
public record GetOrderInspectionsQuery(Guid PurchaseOrderId) : IRequest<Result<List<QualityInspectionDto>>>;
