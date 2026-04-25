using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Verification.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.Commands;

public record SubmitKycCommand(
    string FullName,
    string? NationalId,
    string? PassportNumber,
    string? Nationality,
    DateTime? DateOfBirth,
    string? AddressLine1,
    string? City,
    string? Country,
    string? IdDocumentUrl,
    string? SelfieUrl,
    string? ProofOfAddressUrl) : IRequest<Result<KycVerificationDto>>;

public record ReviewKycCommand(
    Guid KycId,
    VerificationStatus Decision,
    string? RejectionReason) : IRequest<Result>;

public record SubmitKybCommand(
    Guid CompanyId,
    string? CompanyRegistrationDocUrl,
    string? TaxCertificateUrl,
    string? FinancialStatementUrl,
    string? BankStatementUrl,
    string? ProductionLicenseUrl,
    string? ExportLicenseUrl,
    string? FactoryPhotoUrl) : IRequest<Result<KybVerificationDto>>;

public record ReviewKybCommand(
    Guid KybId,
    VerificationStatus Decision,
    string? RejectionReason) : IRequest<Result>;
