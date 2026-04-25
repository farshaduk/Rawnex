using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Disputes.DTOs;

namespace Rawnex.Application.Features.Disputes.Queries;

public record GetDisputeByIdQuery(Guid DisputeId) : IRequest<Result<DisputeDetailDto>>;
public record GetOrderDisputesQuery(Guid PurchaseOrderId) : IRequest<Result<List<DisputeDto>>>;
