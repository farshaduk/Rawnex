using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Orders.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Orders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreatePurchaseOrderCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.BuyerCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of the buyer company.");

        var buyer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);
        if (buyer is null) throw new NotFoundException(nameof(Company), request.BuyerCompanyId);

        var seller = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);
        if (seller is null) throw new NotFoundException(nameof(Company), request.SellerCompanyId);

        var orderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var subTotal = request.Items.Sum(i => i.Quantity * i.UnitPrice);

        var order = new PurchaseOrder
        {
            TenantId = buyer.TenantId,
            OrderNumber = orderNumber,
            BuyerCompanyId = request.BuyerCompanyId,
            SellerCompanyId = request.SellerCompanyId,
            NegotiationId = request.NegotiationId,
            RfqId = request.RfqId,
            Status = OrderStatus.Draft,
            Incoterm = request.Incoterm,
            DeliveryLocation = request.DeliveryLocation,
            RequestedDeliveryDate = request.RequestedDeliveryDate,
            PaymentTerms = request.PaymentTerms,
            SpecialInstructions = request.SpecialInstructions,
            SubTotal = subTotal,
            TotalAmount = subTotal,
            Currency = request.Currency,
        };

        _db.PurchaseOrders.Add(order);

        foreach (var item in request.Items)
        {
            _db.PurchaseOrderItems.Add(new PurchaseOrderItem
            {
                PurchaseOrderId = order.Id,
                ProductId = item.ProductId,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductName,
                Sku = item.Sku,
                Quantity = item.Quantity,
                UnitOfMeasure = item.UnitOfMeasure,
                UnitPrice = item.UnitPrice,
                Currency = request.Currency,
                TotalPrice = item.Quantity * item.UnitPrice,
            });
        }

        await _db.SaveChangesAsync(ct);

        return Result<PurchaseOrderDto>.Success(new PurchaseOrderDto(
            order.Id, order.OrderNumber, order.BuyerCompanyId, buyer.LegalName,
            order.SellerCompanyId, seller.LegalName, order.Status,
            order.TotalAmount, order.Currency, order.Incoterm,
            order.RequestedDeliveryDate, order.CreatedAt));
    }
}

public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ConfirmOrderCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) throw new NotFoundException(nameof(PurchaseOrder), request.OrderId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == order.SellerCompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only seller admins can confirm orders.");

        var oldStatus = order.Status;
        order.Status = OrderStatus.Confirmed;
        order.ConfirmedAt = DateTime.UtcNow;
        order.ConfirmedBy = _currentUser.UserId?.ToString();

        order.AddDomainEvent(new OrderStatusChangedEvent(order.Id, oldStatus, order.Status));
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CancelOrderCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) throw new NotFoundException(nameof(PurchaseOrder), request.OrderId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => (m.CompanyId == order.BuyerCompanyId || m.CompanyId == order.SellerCompanyId)
                && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isMember) throw new ForbiddenAccessException("Only admins of buyer or seller company can cancel.");

        var oldStatus = order.Status;
        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancellationReason = request.Reason;

        order.AddDomainEvent(new OrderStatusChangedEvent(order.Id, oldStatus, order.Status));
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public ChangeOrderStatusCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(ChangeOrderStatusCommand request, CancellationToken ct)
    {
        var order = await _db.PurchaseOrders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);
        if (order is null) throw new NotFoundException(nameof(PurchaseOrder), request.OrderId);

        var oldStatus = order.Status;
        order.Status = request.NewStatus;

        if (request.NewStatus == OrderStatus.Completed)
            order.CompletedAt = DateTime.UtcNow;

        order.AddDomainEvent(new OrderStatusChangedEvent(order.Id, oldStatus, request.NewStatus));
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class ApproveOrderStepCommandHandler : IRequestHandler<ApproveOrderStepCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ApproveOrderStepCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ApproveOrderStepCommand request, CancellationToken ct)
    {
        var approval = await _db.OrderApprovals.FirstOrDefaultAsync(a => a.Id == request.ApprovalId && a.PurchaseOrderId == request.OrderId, ct);
        if (approval is null) throw new NotFoundException(nameof(OrderApproval), request.ApprovalId);

        approval.ApproverUserId = _currentUser.UserId;
        approval.Status = request.Approved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        approval.Comments = request.Comments;
        approval.DecidedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
