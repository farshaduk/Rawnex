using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Ratings.DTOs;
using Rawnex.Domain.Entities;

namespace Rawnex.Application.Features.Ratings.Commands;

public class SubmitRatingCommandHandler : IRequestHandler<SubmitRatingCommand, Result<RatingDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SubmitRatingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<RatingDto>> Handle(SubmitRatingCommand request, CancellationToken ct)
    {
        var reviewer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ReviewerCompanyId, ct);
        if (reviewer is null) throw new NotFoundException(nameof(Company), request.ReviewerCompanyId);

        var reviewed = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.ReviewedCompanyId, ct);

        var existing = await _db.Ratings.AnyAsync(r =>
            r.PurchaseOrderId == request.PurchaseOrderId && r.ReviewerCompanyId == request.ReviewerCompanyId, ct);
        if (existing) return Result<RatingDto>.Failure("You have already rated this order.");

        var rating = new Rating
        {
            TenantId = reviewer.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            ReviewerCompanyId = request.ReviewerCompanyId,
            ReviewerUserId = _currentUser.UserId!.Value,
            ReviewedCompanyId = request.ReviewedCompanyId,
            OverallScore = request.OverallScore,
            QualityScore = request.QualityScore,
            DeliveryScore = request.DeliveryScore,
            CommunicationScore = request.CommunicationScore,
            ValueScore = request.ValueScore,
            Comment = request.Comment,
            IsPublic = request.IsPublic,
        };

        _db.Ratings.Add(rating);
        await _db.SaveChangesAsync(ct);

        return Result<RatingDto>.Success(new RatingDto(
            rating.Id, rating.PurchaseOrderId, rating.ReviewerCompanyId, reviewer.LegalName,
            rating.ReviewedCompanyId, reviewed?.LegalName, rating.OverallScore,
            rating.QualityScore, rating.DeliveryScore, rating.CommunicationScore,
            rating.ValueScore, rating.Comment, rating.IsPublic, null, rating.CreatedAt));
    }
}

public class RespondToRatingCommandHandler : IRequestHandler<RespondToRatingCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RespondToRatingCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RespondToRatingCommand request, CancellationToken ct)
    {
        var rating = await _db.Ratings.FirstOrDefaultAsync(r => r.Id == request.RatingId, ct);
        if (rating is null) throw new NotFoundException(nameof(Rating), request.RatingId);

        var isMember = await _db.CompanyMembers
            .AnyAsync(m => m.CompanyId == rating.ReviewedCompanyId && m.UserId == _currentUser.UserId, ct);
        if (!isMember) throw new ForbiddenAccessException("Only the reviewed company can respond.");

        rating.ResponseComment = request.ResponseComment;
        rating.ResponseAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
