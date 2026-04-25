using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Orders.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Orders.Queries;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<PurchaseOrderDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetOrderByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PurchaseOrderDetailDto>> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var o = await _db.PurchaseOrders.AsNoTracking()
            .Include(x => x.BuyerCompany)
            .Include(x => x.SellerCompany)
            .Include(x => x.Items)
            .Include(x => x.Approvals.OrderBy(a => a.StepOrder))
            .FirstOrDefaultAsync(x => x.Id == request.OrderId, ct);

        if (o is null) throw new NotFoundException(nameof(PurchaseOrder), request.OrderId);

        return Result<PurchaseOrderDetailDto>.Success(new PurchaseOrderDetailDto(
            o.Id, o.TenantId, o.OrderNumber, o.BuyerCompanyId, o.BuyerCompany.LegalName,
            o.SellerCompanyId, o.SellerCompany.LegalName, o.NegotiationId, o.RfqId, o.ContractId,
            o.Status, o.Incoterm, o.DeliveryLocation, o.RequestedDeliveryDate,
            o.PaymentTerms, o.SpecialInstructions, o.SubTotal, o.TaxAmount, o.ShippingCost,
            o.TotalAmount, o.Currency, o.ConfirmedAt, o.CompletedAt, o.CancelledAt, o.CancellationReason,
            o.CreatedAt,
            o.Items.Select(i => new PurchaseOrderItemDto(
                i.Id, i.ProductId, i.ProductName, i.Sku, i.Quantity,
                i.UnitOfMeasure, i.UnitPrice, i.TotalPrice, i.Currency)).ToList(),
            o.Approvals.Select(a => new OrderApprovalDto(
                a.Id, a.StepOrder, a.StepName, a.ApproverUserId,
                a.Status, a.Comments, a.DecidedAt)).ToList()));
    }
}

public class GetCompanyOrdersQueryHandler : IRequestHandler<GetCompanyOrdersQuery, Result<PaginatedList<PurchaseOrderDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyOrdersQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<PurchaseOrderDto>>> Handle(GetCompanyOrdersQuery request, CancellationToken ct)
    {
        var query = _db.PurchaseOrders.AsNoTracking()
            .Include(o => o.BuyerCompany)
            .Include(o => o.SellerCompany)
            .Where(o => o.BuyerCompanyId == request.CompanyId || o.SellerCompanyId == request.CompanyId);

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        var result = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new PurchaseOrderDto(
                o.Id, o.OrderNumber, o.BuyerCompanyId, o.BuyerCompany.LegalName,
                o.SellerCompanyId, o.SellerCompany.LegalName, o.Status,
                o.TotalAmount, o.Currency, o.Incoterm,
                o.RequestedDeliveryDate, o.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<PurchaseOrderDto>>.Success(result);
    }
}
