using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Verification.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Verification.Queries;

public class GetMyKycStatusQueryHandler : IRequestHandler<GetMyKycStatusQuery, Result<KycVerificationDto?>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyKycStatusQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<KycVerificationDto?>> Handle(GetMyKycStatusQuery request, CancellationToken ct)
    {
        var kyc = await _context.KycVerifications
            .Where(k => k.UserId == _currentUser.UserId)
            .OrderByDescending(k => k.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (kyc is null)
            return Result<KycVerificationDto?>.Success(null);

        return Result<KycVerificationDto?>.Success(new KycVerificationDto(
            kyc.Id, kyc.UserId, kyc.Status, kyc.FullName, kyc.Nationality,
            kyc.RejectionReason, kyc.ReviewedAt, kyc.CreatedAt));
    }
}

public class GetPendingKycListQueryHandler : IRequestHandler<GetPendingKycListQuery, Result<PaginatedList<KycVerificationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingKycListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<KycVerificationDto>>> Handle(GetPendingKycListQuery request, CancellationToken ct)
    {
        var query = _context.KycVerifications
            .Where(k => k.Status == VerificationStatus.Pending || k.Status == VerificationStatus.InReview)
            .OrderBy(k => k.CreatedAt)
            .Select(k => new KycVerificationDto(
                k.Id, k.UserId, k.Status, k.FullName, k.Nationality,
                k.RejectionReason, k.ReviewedAt, k.CreatedAt));

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return Result<PaginatedList<KycVerificationDto>>.Success(result);
    }
}

public class GetMyKybStatusQueryHandler : IRequestHandler<GetMyKybStatusQuery, Result<KybVerificationDto?>>
{
    private readonly IApplicationDbContext _context;

    public GetMyKybStatusQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<KybVerificationDto?>> Handle(GetMyKybStatusQuery request, CancellationToken ct)
    {
        var kyb = await _context.KybVerifications
            .Where(k => k.CompanyId == request.CompanyId)
            .OrderByDescending(k => k.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (kyb is null)
            return Result<KybVerificationDto?>.Success(null);

        return Result<KybVerificationDto?>.Success(new KybVerificationDto(
            kyb.Id, kyb.CompanyId, kyb.Status, kyb.RejectionReason, kyb.ReviewedAt, kyb.CreatedAt));
    }
}

public class GetPendingKybListQueryHandler : IRequestHandler<GetPendingKybListQuery, Result<PaginatedList<KybVerificationDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingKybListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<KybVerificationDto>>> Handle(GetPendingKybListQuery request, CancellationToken ct)
    {
        var query = _context.KybVerifications
            .Where(k => k.Status == VerificationStatus.Pending || k.Status == VerificationStatus.InReview)
            .OrderBy(k => k.CreatedAt)
            .Select(k => new KybVerificationDto(
                k.Id, k.CompanyId, k.Status, k.RejectionReason, k.ReviewedAt, k.CreatedAt));

        var result = await query.ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return Result<PaginatedList<KybVerificationDto>>.Success(result);
    }
}
