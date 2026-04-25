using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Contracts.DTOs;

public record ContractDto(
    Guid Id,
    string ContractNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    ContractType Type,
    ContractStatus Status,
    string Title,
    decimal TotalValue,
    Currency Currency,
    DateTime EffectiveDate,
    DateTime? ExpirationDate,
    DateTime CreatedAt
);

public record ContractDetailDto(
    Guid Id,
    Guid TenantId,
    string ContractNumber,
    Guid BuyerCompanyId,
    string? BuyerCompanyName,
    Guid SellerCompanyId,
    string? SellerCompanyName,
    Guid? PurchaseOrderId,
    ContractType Type,
    ContractStatus Status,
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
    DateTime? SignedAt,
    string? DocumentUrl,
    int Version,
    DateTime CreatedAt,
    IList<ContractClauseDto> Clauses,
    IList<DigitalSignatureDto> Signatures
);

public record ContractClauseDto(Guid Id, string Title, string Content, int SortOrder, bool IsStandard);

public record DigitalSignatureDto(
    Guid Id,
    Guid SignerUserId,
    Guid SignerCompanyId,
    string SignerName,
    string SignerRole,
    DateTime SignedAt
);
