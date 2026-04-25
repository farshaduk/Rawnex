using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Workflows.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Workflows.Queries;

public class GetOrderApprovalWorkflowQueryHandler : IRequestHandler<GetOrderApprovalWorkflowQuery, Result<ApprovalWorkflowDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOrderApprovalWorkflowQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<ApprovalWorkflowDto>> Handle(GetOrderApprovalWorkflowQuery request, CancellationToken ct)
    {
        var approvals = await _context.OrderApprovals
            .Where(a => a.PurchaseOrderId == request.PurchaseOrderId)
            .OrderBy(a => a.StepOrder)
            .Select(a => new ApprovalStepDto(
                a.Id, a.StepOrder, a.StepName, a.ApproverUserId,
                a.ApproverUser != null ? a.ApproverUser.FirstName + " " + a.ApproverUser.LastName : null,
                a.Status, a.Comments, a.DecidedAt))
            .ToListAsync(ct);

        if (approvals.Count == 0)
            return Result<ApprovalWorkflowDto>.Failure("No approval workflow found for this order.");

        var isFullyApproved = approvals.All(a => a.Status == ApprovalStatus.Approved);
        return Result<ApprovalWorkflowDto>.Success(new ApprovalWorkflowDto(request.PurchaseOrderId, approvals, isFullyApproved));
    }
}
