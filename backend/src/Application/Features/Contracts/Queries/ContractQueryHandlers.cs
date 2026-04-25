using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Contracts.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Contracts.Queries;

public class GetContractByIdQueryHandler : IRequestHandler<GetContractByIdQuery, Result<ContractDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetContractByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<ContractDetailDto>> Handle(GetContractByIdQuery request, CancellationToken ct)
    {
        var c = await _db.Contracts.AsNoTracking()
            .Include(x => x.BuyerCompany)
            .Include(x => x.SellerCompany)
            .Include(x => x.Clauses.OrderBy(cl => cl.SortOrder))
            .Include(x => x.Signatures)
            .FirstOrDefaultAsync(x => x.Id == request.ContractId, ct);

        if (c is null) throw new NotFoundException(nameof(Contract), request.ContractId);

        return Result<ContractDetailDto>.Success(new ContractDetailDto(
            c.Id, c.TenantId, c.ContractNumber, c.BuyerCompanyId, c.BuyerCompany.LegalName,
            c.SellerCompanyId, c.SellerCompany.LegalName, c.PurchaseOrderId,
            c.Type, c.Status, c.Title, c.Description, c.TotalValue, c.Currency,
            c.Incoterm, c.PaymentTerms, c.DeliveryTerms, c.QualityTerms,
            c.EffectiveDate, c.ExpirationDate, c.SignedAt, c.DocumentUrl, c.Version, c.CreatedAt,
            c.Clauses.Select(cl => new ContractClauseDto(cl.Id, cl.Title, cl.Content, cl.SortOrder, cl.IsStandard)).ToList(),
            c.Signatures.Select(s => new DigitalSignatureDto(
                s.Id, s.SignerUserId, s.SignerCompanyId, s.SignerName, s.SignerRole, s.SignedAt)).ToList()));
    }
}

public class GetCompanyContractsQueryHandler : IRequestHandler<GetCompanyContractsQuery, Result<PaginatedList<ContractDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetCompanyContractsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<PaginatedList<ContractDto>>> Handle(GetCompanyContractsQuery request, CancellationToken ct)
    {
        var result = await _db.Contracts.AsNoTracking()
            .Include(c => c.BuyerCompany)
            .Include(c => c.SellerCompany)
            .Where(c => c.BuyerCompanyId == request.CompanyId || c.SellerCompanyId == request.CompanyId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ContractDto(
                c.Id, c.ContractNumber, c.BuyerCompanyId, c.BuyerCompany.LegalName,
                c.SellerCompanyId, c.SellerCompany.LegalName, c.Type, c.Status,
                c.Title, c.TotalValue, c.Currency, c.EffectiveDate, c.ExpirationDate, c.CreatedAt))
            .ToPaginatedListAsync(request.PageNumber, request.PageSize, ct);

        return Result<PaginatedList<ContractDto>>.Success(result);
    }
}
