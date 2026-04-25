using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Payments.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Payments.Queries;

public class GetEscrowByOrderQueryHandler : IRequestHandler<GetEscrowByOrderQuery, Result<EscrowAccountDto>>
{
    private readonly IApplicationDbContext _db;

    public GetEscrowByOrderQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EscrowAccountDto>> Handle(GetEscrowByOrderQuery request, CancellationToken ct)
    {
        var e = await _db.EscrowAccounts.AsNoTracking()
            .Include(x => x.BuyerCompany)
            .Include(x => x.SellerCompany)
            .FirstOrDefaultAsync(x => x.PurchaseOrderId == request.PurchaseOrderId, ct);

        if (e is null) throw new NotFoundException(nameof(EscrowAccount), request.PurchaseOrderId);

        return Result<EscrowAccountDto>.Success(new EscrowAccountDto(
            e.Id, e.PurchaseOrderId, e.BuyerCompanyId, e.BuyerCompany.LegalName,
            e.SellerCompanyId, e.SellerCompany.LegalName, e.Status,
            e.TotalAmount, e.FundedAmount, e.ReleasedAmount, e.Currency, e.CreatedAt));
    }
}

public class GetPaymentsByOrderQueryHandler : IRequestHandler<GetPaymentsByOrderQuery, Result<List<PaymentDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetPaymentsByOrderQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<List<PaymentDto>>> Handle(GetPaymentsByOrderQuery request, CancellationToken ct)
    {
        var payments = await _db.Payments.AsNoTracking()
            .Include(p => p.PayerCompany)
            .Include(p => p.PayeeCompany)
            .Where(p => p.PurchaseOrderId == request.PurchaseOrderId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PaymentDto(
                p.Id, p.PayerCompanyId, p.PayerCompany.LegalName,
                p.PayeeCompanyId, p.PayeeCompany.LegalName, p.PaymentReference,
                p.Method, p.Status, p.Amount, p.Currency, p.ProcessedAt, p.CreatedAt))
            .ToListAsync(ct);

        return Result<List<PaymentDto>>.Success(payments);
    }
}

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetInvoiceByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<InvoiceDetailDto>> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        var i = await _db.Invoices.AsNoTracking()
            .Include(x => x.IssuerCompany)
            .Include(x => x.RecipientCompany)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, ct);

        if (i is null) throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        return Result<InvoiceDetailDto>.Success(new InvoiceDetailDto(
            i.Id, i.TenantId, i.InvoiceNumber, i.PurchaseOrderId,
            i.IssuerCompanyId, i.IssuerCompany.LegalName,
            i.RecipientCompanyId, i.RecipientCompany.LegalName,
            i.Type, i.Status, i.IssueDate, i.DueDate,
            i.SubTotal, i.TaxAmount, i.DiscountAmount, i.TotalAmount, i.PaidAmount,
            i.Currency, i.DocumentUrl, i.Notes, i.PaidAt, i.CreatedAt,
            i.Items.Select(item => new InvoiceItemDto(
                item.Id, item.Description, item.Quantity, item.UnitOfMeasure,
                item.UnitPrice, item.TotalPrice)).ToList()));
    }
}

public class GetCompanyInvoicesQueryHandler : IRequestHandler<GetCompanyInvoicesQuery, Result<PaginatedList<InvoiceDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyInvoicesQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<InvoiceDto>>> Handle(GetCompanyInvoicesQuery request, CancellationToken ct)
    {
        var result = await _db.Invoices.AsNoTracking()
            .Include(i => i.IssuerCompany)
            .Include(i => i.RecipientCompany)
            .Where(i => i.IssuerCompanyId == request.CompanyId || i.RecipientCompanyId == request.CompanyId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvoiceDto(
                i.Id, i.InvoiceNumber, i.IssuerCompanyId, i.IssuerCompany.LegalName,
                i.RecipientCompanyId, i.RecipientCompany.LegalName, i.Type, i.Status,
                i.IssueDate, i.DueDate, i.TotalAmount, i.PaidAmount, i.Currency, i.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<InvoiceDto>>.Success(result);
    }
}
