using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Contracts.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Contracts.Commands;

public record CreateContractCommand(
    Guid BuyerCompanyId,
    Guid SellerCompanyId,
    Guid? PurchaseOrderId,
    ContractType Type,
    string Title,
    string? Description,
    decimal TotalValue,
    Currency Currency,
    Incoterm? Incoterm,
    string? PaymentTerms,
    string? DeliveryTerms,
    string? QualityTerms,
    DateTime EffectiveDate,
    DateTime? ExpirationDate,
    List<CreateClauseDto>? Clauses
) : IRequest<Result<ContractDto>>;

public record CreateClauseDto(string Title, string Content, int SortOrder, bool IsStandard = false);

public record SignContractCommand(
    Guid ContractId,
    Guid SignerCompanyId,
    string SignerName,
    string SignerRole,
    string SignatureHash
) : IRequest<Result>;

public record TerminateContractCommand(
    Guid ContractId,
    string Reason
) : IRequest<Result>;
