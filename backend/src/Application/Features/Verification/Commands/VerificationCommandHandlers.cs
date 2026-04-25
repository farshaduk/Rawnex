using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Verification.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.Commands;

public class SubmitKycCommandHandler : IRequestHandler<SubmitKycCommand, Result<KycVerificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SubmitKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<KycVerificationDto>> Handle(SubmitKycCommand request, CancellationToken ct)
    {
        var existing = await _context.KycVerifications
            .FirstOrDefaultAsync(k => k.UserId == _currentUser.UserId && k.Status == VerificationStatus.Pending, ct);
        if (existing != null)
            return Result<KycVerificationDto>.Failure("A KYC verification is already pending.");

        var kyc = new KycVerification
        {
            UserId = _currentUser.UserId!.Value,
            TenantId = Guid.Empty, // Will be set by interceptor or current context
            Status = VerificationStatus.Pending,
            FullName = request.FullName,
            NationalId = request.NationalId,
            PassportNumber = request.PassportNumber,
            Nationality = request.Nationality,
            DateOfBirth = request.DateOfBirth,
            AddressLine1 = request.AddressLine1,
            City = request.City,
            Country = request.Country,
            IdDocumentUrl = request.IdDocumentUrl,
            SelfieUrl = request.SelfieUrl,
            ProofOfAddressUrl = request.ProofOfAddressUrl,
        };

        _context.KycVerifications.Add(kyc);
        await _context.SaveChangesAsync(ct);

        return Result<KycVerificationDto>.Success(new KycVerificationDto(
            kyc.Id, kyc.UserId, kyc.Status, kyc.FullName, kyc.Nationality, null, null, kyc.CreatedAt));
    }
}

public class ReviewKycCommandHandler : IRequestHandler<ReviewKycCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public ReviewKycCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(ReviewKycCommand request, CancellationToken ct)
    {
        var kyc = await _context.KycVerifications.FindAsync(new object[] { request.KycId }, ct);
        if (kyc is null) return Result.Failure("KYC verification not found.");
        if (kyc.Status != VerificationStatus.Pending && kyc.Status != VerificationStatus.InReview)
            return Result.Failure("KYC is not in a reviewable state.");

        kyc.Status = request.Decision;
        kyc.RejectionReason = request.Decision == VerificationStatus.Rejected ? request.RejectionReason : null;
        kyc.ReviewedBy = _currentUser.UserId.ToString();
        kyc.ReviewedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);

        var title = request.Decision == VerificationStatus.Approved ? "KYC Approved" : "KYC Rejected";
        var message = request.Decision == VerificationStatus.Approved
            ? "Your identity verification has been approved."
            : $"Your identity verification was rejected: {request.RejectionReason}";

        await _notificationService.SendAsync(kyc.UserId, kyc.TenantId, title, message,
            NotificationType.System, NotificationPriority.High, ct: ct);

        return Result.Success();
    }
}

public class SubmitKybCommandHandler : IRequestHandler<SubmitKybCommand, Result<KybVerificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SubmitKybCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<KybVerificationDto>> Handle(SubmitKybCommand request, CancellationToken ct)
    {
        var company = await _context.Companies.FindAsync(new object[] { request.CompanyId }, ct);
        if (company is null) return Result<KybVerificationDto>.Failure("Company not found.");

        var existing = await _context.KybVerifications
            .FirstOrDefaultAsync(k => k.CompanyId == request.CompanyId && k.Status == VerificationStatus.Pending, ct);
        if (existing != null)
            return Result<KybVerificationDto>.Failure("A KYB verification is already pending for this company.");

        var kyb = new KybVerification
        {
            TenantId = company.TenantId,
            CompanyId = request.CompanyId,
            Status = VerificationStatus.Pending,
            CompanyRegistrationDocUrl = request.CompanyRegistrationDocUrl,
            TaxCertificateUrl = request.TaxCertificateUrl,
            FinancialStatementUrl = request.FinancialStatementUrl,
            BankStatementUrl = request.BankStatementUrl,
            ProductionLicenseUrl = request.ProductionLicenseUrl,
            ExportLicenseUrl = request.ExportLicenseUrl,
            FactoryPhotoUrl = request.FactoryPhotoUrl,
        };

        _context.KybVerifications.Add(kyb);
        await _context.SaveChangesAsync(ct);

        return Result<KybVerificationDto>.Success(new KybVerificationDto(
            kyb.Id, kyb.CompanyId, kyb.Status, null, null, kyb.CreatedAt));
    }
}

public class ReviewKybCommandHandler : IRequestHandler<ReviewKybCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public ReviewKybCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(ReviewKybCommand request, CancellationToken ct)
    {
        var kyb = await _context.KybVerifications.FindAsync(new object[] { request.KybId }, ct);
        if (kyb is null) return Result.Failure("KYB verification not found.");
        if (kyb.Status != VerificationStatus.Pending && kyb.Status != VerificationStatus.InReview)
            return Result.Failure("KYB is not in a reviewable state.");

        kyb.Status = request.Decision;
        kyb.RejectionReason = request.Decision == VerificationStatus.Rejected ? request.RejectionReason : null;
        kyb.ReviewedBy = _currentUser.UserId.ToString();
        kyb.ReviewedAt = DateTime.UtcNow;

        if (request.Decision == VerificationStatus.Approved)
        {
            var company = await _context.Companies.FindAsync(new object[] { kyb.CompanyId }, ct);
            if (company != null)
            {
                company.VerificationStatus = VerificationStatus.Approved;
                company.VerifiedAt = DateTime.UtcNow;
                company.VerifiedBy = _currentUser.UserId.ToString();
            }
        }

        await _context.SaveChangesAsync(ct);

        await _notificationService.SendToCompanyAsync(kyb.CompanyId,
            request.Decision == VerificationStatus.Approved ? "KYB Approved" : "KYB Rejected",
            request.Decision == VerificationStatus.Approved
                ? "Your company verification has been approved."
                : $"Your company verification was rejected: {request.RejectionReason}",
            NotificationType.System, NotificationPriority.High, ct: ct);

        return Result.Success();
    }
}
