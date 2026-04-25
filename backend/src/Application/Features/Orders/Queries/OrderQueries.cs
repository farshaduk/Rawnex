using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Orders.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<PurchaseOrderDetailDto>>;

public record GetCompanyOrdersQuery(
    Guid CompanyId,
    OrderStatus? Status,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedList<PurchaseOrderDto>>>;
