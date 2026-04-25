using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Companies.DTOs;

public record CompanyDto(
    Guid Id,
    Guid TenantId,
    string LegalName,
    string? TradeName,
    string? RegistrationNumber,
    CompanyType Type,
    CompanyStatus Status,
    VerificationStatus VerificationStatus,
    string? City,
    string? Country,
    string? Website,
    decimal? TrustScore,
    decimal? EsgScore,
    DateTime CreatedAt
);

public record CompanyDetailDto(
    Guid Id,
    Guid TenantId,
    string LegalName,
    string? TradeName,
    string? RegistrationNumber,
    string? TaxId,
    CompanyType Type,
    CompanyStatus Status,
    VerificationStatus VerificationStatus,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? Country,
    string? PostalCode,
    string? Phone,
    string? Email,
    string? Website,
    string? LogoUrl,
    string? BankName,
    string? BankAccountNumber,
    string? BankSwiftCode,
    string? BankIban,
    decimal? TrustScore,
    decimal? EsgScore,
    Guid? ParentCompanyId,
    DateTime CreatedAt,
    IList<CompanyMemberDto> Members,
    IList<CompanyDocumentDto> Documents,
    IList<DepartmentDto> Departments
);

public record CompanyMemberDto(
    Guid Id,
    Guid UserId,
    string UserEmail,
    string UserName,
    Guid? DepartmentId,
    string? DepartmentName,
    string? JobTitle,
    bool IsCompanyAdmin,
    DateTime CreatedAt
);

public record CompanyDocumentDto(
    Guid Id,
    DocumentType Type,
    string? FileName,
    string FileUrl,
    VerificationStatus VerificationStatus,
    DateTime? ExpiresAt,
    DateTime CreatedAt
);

public record DepartmentDto(
    Guid Id,
    string Name,
    DepartmentType Type,
    Guid? ParentDepartmentId,
    string? ManagerName
);
