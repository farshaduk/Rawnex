using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Contracts.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Contracts.Commands;

public class CreateContractCommandHandler : IRequestHandler<CreateContractCommand, Result<ContractDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateContractCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<ContractDto>> Handle(CreateContractCommand request, CancellationToken ct)
    {
        var buyer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.BuyerCompanyId, ct);
        if (buyer is null) throw new NotFoundException(nameof(Company), request.BuyerCompanyId);

        var seller = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.SellerCompanyId, ct);
        if (seller is null) throw new NotFoundException(nameof(Company), request.SellerCompanyId);

        var contractNumber = $"CTR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var contract = new Contract
        {
            TenantId = buyer.TenantId,
            ContractNumber = contractNumber,
            BuyerCompanyId = request.BuyerCompanyId,
            SellerCompanyId = request.SellerCompanyId,
            PurchaseOrderId = request.PurchaseOrderId,
            Type = request.Type,
            Status = ContractStatus.Draft,
            Title = request.Title,
            Description = request.Description,
            TotalValue = request.TotalValue,
            Currency = request.Currency,
            Incoterm = request.Incoterm,
            PaymentTerms = request.PaymentTerms,
            DeliveryTerms = request.DeliveryTerms,
            QualityTerms = request.QualityTerms,
            EffectiveDate = request.EffectiveDate,
            ExpirationDate = request.ExpirationDate,
        };

        _db.Contracts.Add(contract);

        if (request.Clauses is not null)
        {
            foreach (var clause in request.Clauses)
            {
                _db.ContractClauses.Add(new ContractClause
                {
                    ContractId = contract.Id,
                    Title = clause.Title,
                    Content = clause.Content,
                    SortOrder = clause.SortOrder,
                    IsStandard = clause.IsStandard,
                });
            }
        }

        await _db.SaveChangesAsync(ct);

        return Result<ContractDto>.Success(new ContractDto(
            contract.Id, contract.ContractNumber, contract.BuyerCompanyId, buyer.LegalName,
            contract.SellerCompanyId, seller.LegalName, contract.Type, contract.Status,
            contract.Title, contract.TotalValue, contract.Currency,
            contract.EffectiveDate, contract.ExpirationDate, contract.CreatedAt));
    }
}

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SignContractCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(SignContractCommand request, CancellationToken ct)
    {
        var contract = await _db.Contracts
            .Include(c => c.Signatures)
            .FirstOrDefaultAsync(c => c.Id == request.ContractId, ct);
        if (contract is null) throw new NotFoundException(nameof(Contract), request.ContractId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == request.SignerCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Not a member of this company.");

        var alreadySigned = contract.Signatures.Any(s => s.SignerCompanyId == request.SignerCompanyId);
        if (alreadySigned) return Result.Failure("This company has already signed the contract.");

        _db.DigitalSignatures.Add(new DigitalSignature
        {
            ContractId = request.ContractId,
            SignerUserId = _currentUser.UserId!.Value,
            SignerCompanyId = request.SignerCompanyId,
            SignerName = request.SignerName,
            SignerRole = request.SignerRole,
            SignatureHash = request.SignatureHash,
        });

        // If both parties have signed, mark as active
        var signatories = contract.Signatures.Select(s => s.SignerCompanyId).ToHashSet();
        signatories.Add(request.SignerCompanyId);
        if (signatories.Contains(contract.BuyerCompanyId) && signatories.Contains(contract.SellerCompanyId))
        {
            contract.Status = ContractStatus.Active;
            contract.SignedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class TerminateContractCommandHandler : IRequestHandler<TerminateContractCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public TerminateContractCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(TerminateContractCommand request, CancellationToken ct)
    {
        var contract = await _db.Contracts.FirstOrDefaultAsync(c => c.Id == request.ContractId, ct);
        if (contract is null) throw new NotFoundException(nameof(Contract), request.ContractId);

        contract.Status = ContractStatus.Terminated;
        contract.TerminatedAt = DateTime.UtcNow;
        contract.TerminationReason = request.Reason;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
