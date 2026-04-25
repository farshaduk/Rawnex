using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Rfqs.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Rfqs.Commands;

public class CreateRfqCommandHandler : IRequestHandler<CreateRfqCommand, Result<RfqDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateRfqCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<RfqDto>> Handle(CreateRfqCommand request, CancellationToken ct)
    {
        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.BuyerCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);
        if (company is null) throw new NotFoundException(nameof(Company), request.BuyerCompanyId);

        var rfqNumber = $"RFQ-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var rfq = new Rfq
        {
            TenantId = company.TenantId,
            BuyerCompanyId = request.BuyerCompanyId,
            RfqNumber = rfqNumber,
            Title = request.Title,
            Description = request.Description,
            Status = RfqStatus.Draft,
            Visibility = request.Visibility,
            CategoryId = request.CategoryId,
            MaterialName = request.MaterialName,
            RequiredSpecsJson = request.RequiredSpecsJson,
            RequiredQuantity = request.RequiredQuantity,
            UnitOfMeasure = request.UnitOfMeasure,
            BudgetMin = request.BudgetMin,
            BudgetMax = request.BudgetMax,
            BudgetCurrency = request.BudgetCurrency,
            PreferredIncoterm = request.PreferredIncoterm,
            DeliveryLocation = request.DeliveryLocation,
            DeliveryDeadline = request.DeliveryDeadline,
            ResponseDeadline = request.ResponseDeadline,
        };

        rfq.AddDomainEvent(new RfqPublishedEvent(rfq.Id, rfq.BuyerCompanyId));

        _db.Rfqs.Add(rfq);
        await _db.SaveChangesAsync(ct);

        return Result<RfqDto>.Success(new RfqDto(
            rfq.Id, rfq.RfqNumber, rfq.BuyerCompanyId, company.LegalName,
            rfq.Title, rfq.Status, rfq.Visibility, rfq.MaterialName,
            rfq.RequiredQuantity, rfq.BudgetCurrency, rfq.ResponseDeadline, 0, rfq.CreatedAt));
    }
}

public class SubmitRfqResponseCommandHandler : IRequestHandler<SubmitRfqResponseCommand, Result<RfqResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SubmitRfqResponseCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<RfqResponseDto>> Handle(SubmitRfqResponseCommand request, CancellationToken ct)
    {
        var rfq = await _db.Rfqs.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RfqId, ct);
        if (rfq is null) throw new NotFoundException(nameof(Rfq), request.RfqId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.SellerCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var company = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);

        var response = new RfqResponse
        {
            TenantId = rfq.TenantId,
            RfqId = request.RfqId,
            SellerCompanyId = request.SellerCompanyId,
            Status = BidStatus.Submitted,
            ProposedPrice = request.ProposedPrice,
            PriceCurrency = request.PriceCurrency,
            PriceUnit = request.PriceUnit,
            ProposedQuantity = request.ProposedQuantity,
            UnitOfMeasure = request.UnitOfMeasure,
            Incoterm = request.Incoterm,
            LeadTimeDays = request.LeadTimeDays,
            PaymentTerms = request.PaymentTerms,
            TechnicalSpecsJson = request.TechnicalSpecsJson,
            Notes = request.Notes,
            ValidUntil = request.ValidUntil,
        };

        _db.RfqResponses.Add(response);
        await _db.SaveChangesAsync(ct);

        return Result<RfqResponseDto>.Success(new RfqResponseDto(
            response.Id, response.SellerCompanyId, company?.LegalName,
            response.Status, response.ProposedPrice, response.PriceCurrency,
            response.ProposedQuantity, response.Incoterm, response.LeadTimeDays,
            response.PaymentTerms, response.Notes, response.ValidUntil, response.CreatedAt));
    }
}

public class AwardRfqCommandHandler : IRequestHandler<AwardRfqCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AwardRfqCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(AwardRfqCommand request, CancellationToken ct)
    {
        var rfq = await _db.Rfqs.FirstOrDefaultAsync(r => r.Id == request.RfqId, ct);
        if (rfq is null) throw new NotFoundException(nameof(Rfq), request.RfqId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == rfq.BuyerCompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only buyer company admins can award RFQs.");

        rfq.Status = RfqStatus.Awarded;
        rfq.AwardedToCompanyId = request.AwardedToCompanyId;
        rfq.AwardedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class CancelRfqCommandHandler : IRequestHandler<CancelRfqCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CancelRfqCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(CancelRfqCommand request, CancellationToken ct)
    {
        var rfq = await _db.Rfqs.FirstOrDefaultAsync(r => r.Id == request.RfqId, ct);
        if (rfq is null) throw new NotFoundException(nameof(Rfq), request.RfqId);

        var isAdmin = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == rfq.BuyerCompanyId && m.UserId == _currentUser.UserId && m.IsCompanyAdmin, ct);
        if (!isAdmin) throw new ForbiddenAccessException("Only buyer company admins can cancel RFQs.");

        rfq.Status = RfqStatus.Cancelled;
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
