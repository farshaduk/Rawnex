using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Payments.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Payments.Commands;

public class CreateEscrowAccountCommandHandler : IRequestHandler<CreateEscrowAccountCommand, Result<EscrowAccountDto>>
{
    private readonly IApplicationDbContext _db;

    public CreateEscrowAccountCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EscrowAccountDto>> Handle(CreateEscrowAccountCommand request, CancellationToken ct)
    {
        var buyer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);
        var seller = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);

        var escrow = new EscrowAccount
        {
            TenantId = buyer!.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            BuyerCompanyId = request.BuyerCompanyId,
            SellerCompanyId = request.SellerCompanyId,
            Status = EscrowStatus.Created,
            TotalAmount = request.TotalAmount,
            Currency = request.Currency,
        };

        _db.EscrowAccounts.Add(escrow);

        if (request.Milestones is not null)
        {
            foreach (var m in request.Milestones)
            {
                _db.EscrowMilestones.Add(new EscrowMilestone
                {
                    EscrowAccountId = escrow.Id,
                    Type = m.Type,
                    Status = MilestoneStatus.Pending,
                    Description = m.Description,
                    ReleasePercentage = m.ReleasePercentage,
                    ReleaseAmount = request.TotalAmount * m.ReleasePercentage / 100m,
                    SortOrder = m.SortOrder,
                });
            }
        }

        await _db.SaveChangesAsync(ct);

        return Result<EscrowAccountDto>.Success(new EscrowAccountDto(
            escrow.Id, escrow.PurchaseOrderId, escrow.BuyerCompanyId, buyer.LegalName,
            escrow.SellerCompanyId, seller?.LegalName, escrow.Status,
            escrow.TotalAmount, escrow.FundedAmount, escrow.ReleasedAmount,
            escrow.Currency, escrow.CreatedAt));
    }
}

public class FundEscrowCommandHandler : IRequestHandler<FundEscrowCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public FundEscrowCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(FundEscrowCommand request, CancellationToken ct)
    {
        var escrow = await _db.EscrowAccounts.FirstOrDefaultAsync(e => e.Id == request.EscrowAccountId, ct);
        if (escrow is null) throw new NotFoundException(nameof(EscrowAccount), request.EscrowAccountId);

        escrow.FundedAmount += request.Amount;
        if (escrow.FundedAmount >= escrow.TotalAmount)
        {
            escrow.Status = EscrowStatus.Funded;
            escrow.FundedAt = DateTime.UtcNow;
            escrow.AddDomainEvent(new EscrowFundedEvent(escrow.Id, escrow.PurchaseOrderId, escrow.FundedAmount));
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CompleteMilestoneCommandHandler : IRequestHandler<CompleteMilestoneCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CompleteMilestoneCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CompleteMilestoneCommand request, CancellationToken ct)
    {
        var escrow = await _db.EscrowAccounts
            .Include(e => e.Milestones)
            .FirstOrDefaultAsync(e => e.Id == request.EscrowAccountId, ct);
        if (escrow is null) throw new NotFoundException(nameof(EscrowAccount), request.EscrowAccountId);

        var milestone = escrow.Milestones.FirstOrDefault(m => m.Id == request.MilestoneId);
        if (milestone is null) throw new NotFoundException(nameof(EscrowMilestone), request.MilestoneId);

        milestone.Status = MilestoneStatus.Completed;
        milestone.CompletedAt = DateTime.UtcNow;
        milestone.CompletedBy = _currentUser.UserId?.ToString();
        milestone.EvidenceUrl = request.EvidenceUrl;
        milestone.Notes = request.Notes;

        escrow.ReleasedAmount += milestone.ReleaseAmount;
        milestone.AddDomainEvent(new EscrowMilestoneCompletedEvent(escrow.Id, milestone.Id, milestone.ReleaseAmount));

        if (escrow.Milestones.All(m => m.Status == MilestoneStatus.Completed))
        {
            escrow.Status = EscrowStatus.Released;
            escrow.FullyReleasedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, Result<PaymentDto>>
{
    private readonly IApplicationDbContext _db;

    public RecordPaymentCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaymentDto>> Handle(RecordPaymentCommand request, CancellationToken ct)
    {
        var payer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.PayerCompanyId, ct);
        var payee = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.PayeeCompanyId, ct);

        var paymentRef = $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var payment = new Payment
        {
            TenantId = payer!.TenantId,
            EscrowAccountId = request.EscrowAccountId,
            PurchaseOrderId = request.PurchaseOrderId,
            PayerCompanyId = request.PayerCompanyId,
            PayeeCompanyId = request.PayeeCompanyId,
            PaymentReference = paymentRef,
            Method = request.Method,
            Status = PaymentStatus.Pending,
            Amount = request.Amount,
            Currency = request.Currency,
            TransactionId = request.TransactionId,
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync(ct);

        return Result<PaymentDto>.Success(new PaymentDto(
            payment.Id, payment.PayerCompanyId, payer.LegalName,
            payment.PayeeCompanyId, payee?.LegalName, payment.PaymentReference,
            payment.Method, payment.Status, payment.Amount, payment.Currency,
            payment.ProcessedAt, payment.CreatedAt));
    }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IApplicationDbContext _db;

    public CreateInvoiceCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<InvoiceDto>> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        var issuer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.IssuerCompanyId, ct);
        var recipient = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.RecipientCompanyId, ct);

        var invoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var subTotal = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        var totalAmount = subTotal + (request.TaxAmount ?? 0) - (request.DiscountAmount ?? 0);

        var invoice = new Invoice
        {
            TenantId = issuer!.TenantId,
            InvoiceNumber = invoiceNumber,
            PurchaseOrderId = request.PurchaseOrderId,
            IssuerCompanyId = request.IssuerCompanyId,
            RecipientCompanyId = request.RecipientCompanyId,
            Type = request.Type,
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow,
            DueDate = request.DueDate,
            SubTotal = subTotal,
            TaxAmount = request.TaxAmount,
            DiscountAmount = request.DiscountAmount,
            TotalAmount = totalAmount,
            Currency = request.Currency,
            Notes = request.Notes,
        };

        _db.Invoices.Add(invoice);

        foreach (var item in request.Items)
        {
            _db.InvoiceItems.Add(new InvoiceItem
            {
                InvoiceId = invoice.Id,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitOfMeasure = item.UnitOfMeasure,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice,
                Currency = request.Currency,
            });
        }

        await _db.SaveChangesAsync(ct);

        return Result<InvoiceDto>.Success(new InvoiceDto(
            invoice.Id, invoice.InvoiceNumber, invoice.IssuerCompanyId, issuer.LegalName,
            invoice.RecipientCompanyId, recipient?.LegalName, invoice.Type, invoice.Status,
            invoice.IssueDate, invoice.DueDate, invoice.TotalAmount, invoice.PaidAmount,
            invoice.Currency, invoice.CreatedAt));
    }
}

public class MarkInvoicePaidCommandHandler : IRequestHandler<MarkInvoicePaidCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public MarkInvoicePaidCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(MarkInvoicePaidCommand request, CancellationToken ct)
    {
        var invoice = await _db.Invoices.FirstOrDefaultAsync(i => i.Id == request.InvoiceId, ct);
        if (invoice is null) throw new NotFoundException(nameof(Invoice), request.InvoiceId);

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAmount = invoice.TotalAmount;
        invoice.PaidAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
