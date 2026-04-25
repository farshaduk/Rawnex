using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Companies.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Companies.Commands;

public record RegisterCompanyCommand(
    string LegalName,
    string? TradeName,
    string? RegistrationNumber,
    string? TaxId,
    CompanyType Type,
    string? AddressLine1,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? Website
) : IRequest<Result<CompanyDto>>;

public record UpdateCompanyCommand(
    Guid CompanyId,
    string? TradeName,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? Website,
    string? BankName,
    string? BankAccountNumber,
    string? BankSwiftCode,
    string? BankIban
) : IRequest<Result>;

public record VerifyCompanyCommand(
    Guid CompanyId,
    VerificationStatus NewStatus,
    string? Notes
) : IRequest<Result>;

public record AddCompanyMemberCommand(
    Guid CompanyId,
    Guid UserId,
    Guid? DepartmentId,
    string? JobTitle,
    bool IsAdmin
) : IRequest<Result>;

public record RemoveCompanyMemberCommand(
    Guid CompanyId,
    Guid UserId
) : IRequest<Result>;

public record CreateDepartmentCommand(
    Guid CompanyId,
    string Name,
    DepartmentType Type,
    Guid? ParentDepartmentId,
    Guid? ManagerUserId
) : IRequest<Result<DepartmentDto>>;

public record UploadCompanyDocumentCommand(
    Guid CompanyId,
    DocumentType Type,
    string FileName,
    string FileUrl,
    DateTime? ExpiresAt
) : IRequest<Result<CompanyDocumentDto>>;
