using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Exceptions;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Disputes.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;
using Rawnex.Domain.Events;

namespace Rawnex.Application.Features.Disputes.Commands;

public class FileDisputeCommandHandler : IRequestHandler<FileDisputeCommand, Result<DisputeDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public FileDisputeCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<DisputeDto>> Handle(FileDisputeCommand request, CancellationToken ct)
    {
        var filer = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.FiledByCompanyId, ct);
        if (filer is null) throw new NotFoundException(nameof(Company), request.FiledByCompanyId);

        var against = await _db.Companies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == request.AgainstCompanyId, ct);

        var disputeNumber = $"DSP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}";

        var dispute = new Dispute
        {
            TenantId = filer.TenantId,
            PurchaseOrderId = request.PurchaseOrderId,
            FiledByCompanyId = request.FiledByCompanyId,
            FiledByUserId = _currentUser.UserId!.Value,
            AgainstCompanyId = request.AgainstCompanyId,
            DisputeNumber = disputeNumber,
            Status = DisputeStatus.Filed,
            Reason = request.Reason,
            Description = request.Description,
            ClaimedAmount = request.ClaimedAmount,
            ClaimedCurrency = request.ClaimedCurrency,
        };

        dispute.AddDomainEvent(new DisputeFiledEvent(dispute.Id, dispute.PurchaseOrderId, dispute.FiledByCompanyId));

        _db.Disputes.Add(dispute);
        await _db.SaveChangesAsync(ct);

        return Result<DisputeDto>.Success(new DisputeDto(
            dispute.Id, dispute.DisputeNumber, dispute.PurchaseOrderId,
            dispute.FiledByCompanyId, filer.LegalName,
            dispute.AgainstCompanyId, against?.LegalName,
            dispute.Status, dispute.Reason, dispute.ClaimedAmount, dispute.ClaimedCurrency,
            dispute.CreatedAt));
    }
}

public class AddDisputeEvidenceCommandHandler : IRequestHandler<AddDisputeEvidenceCommand, Result<DisputeEvidenceDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddDisputeEvidenceCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<DisputeEvidenceDto>> Handle(AddDisputeEvidenceCommand request, CancellationToken ct)
    {
        var dispute = await _db.Disputes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);
        if (dispute is null) throw new NotFoundException(nameof(Dispute), request.DisputeId);

        var evidence = new DisputeEvidence
        {
            DisputeId = request.DisputeId,
            UploadedByUserId = _currentUser.UserId!.Value,
            Title = request.Title,
            Description = request.Description,
            FileUrl = request.FileUrl,
            MimeType = request.MimeType,
            FileSizeBytes = request.FileSizeBytes,
        };

        _db.DisputeEvidences.Add(evidence);
        await _db.SaveChangesAsync(ct);

        return Result<DisputeEvidenceDto>.Success(new DisputeEvidenceDto(
            evidence.Id, evidence.UploadedByUserId, evidence.Title, evidence.Description,
            evidence.FileUrl, evidence.MimeType, evidence.FileSizeBytes, evidence.CreatedAt));
    }
}

public class ResolveDisputeCommandHandler : IRequestHandler<ResolveDisputeCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ResolveDisputeCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ResolveDisputeCommand request, CancellationToken ct)
    {
        var dispute = await _db.Disputes.FirstOrDefaultAsync(d => d.Id == request.DisputeId, ct);
        if (dispute is null) throw new NotFoundException(nameof(Dispute), request.DisputeId);

        dispute.Status = DisputeStatus.Resolved;
        dispute.Resolution = request.Resolution;
        dispute.ResolutionNotes = request.ResolutionNotes;
        dispute.ResolvedAmount = request.ResolvedAmount;
        dispute.ResolvedAt = DateTime.UtcNow;
        dispute.ResolvedBy = _currentUser.UserId?.ToString();

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
