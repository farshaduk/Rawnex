using MediatR;
using Microsoft.EntityFrameworkCore;
using Rawnex.Application.Common.Interfaces;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Workflows.DTOs;
using Rawnex.Domain.Entities;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Workflows.Commands;

public class InitiateApprovalWorkflowCommandHandler : IRequestHandler<InitiateApprovalWorkflowCommand, Result<ApprovalWorkflowDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public InitiateApprovalWorkflowCommandHandler(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<Result<ApprovalWorkflowDto>> Handle(InitiateApprovalWorkflowCommand request, CancellationToken ct)
    {
        var order = await _context.PurchaseOrders.FindAsync(new object[] { request.PurchaseOrderId }, ct);
        if (order is null) return Result<ApprovalWorkflowDto>.Failure("Purchase order not found.");

        var existing = await _context.OrderApprovals
            .AnyAsync(a => a.PurchaseOrderId == request.PurchaseOrderId, ct);
        if (existing) return Result<ApprovalWorkflowDto>.Failure("Approval workflow already exists for this order.");

        var approvals = request.Steps.OrderBy(s => s.StepOrder).Select(s => new OrderApproval
        {
            PurchaseOrderId = request.PurchaseOrderId,
            StepOrder = s.StepOrder,
            StepName = s.StepName,
            ApproverUserId = s.ApproverUserId,
            Status = ApprovalStatus.Pending,
        }).ToList();

        _context.OrderApprovals.AddRange(approvals);
        order.Status = OrderStatus.PendingApproval;
        await _context.SaveChangesAsync(ct);

        // Notify first approver
        var firstStep = approvals.First();
        if (firstStep.ApproverUserId.HasValue)
        {
            await _notificationService.SendAsync(firstStep.ApproverUserId.Value, order.TenantId,
                "Approval Required", $"Order approval required: {firstStep.StepName}",
                NotificationType.ApprovalRequired, NotificationPriority.High, ct: ct);
        }

        var steps = approvals.Select(a => new ApprovalStepDto(
            a.Id, a.StepOrder, a.StepName, a.ApproverUserId, null, a.Status, a.Comments, a.DecidedAt)).ToList();

        return Result<ApprovalWorkflowDto>.Success(new ApprovalWorkflowDto(order.Id, steps, false));
    }
}

public class DecideApprovalStepCommandHandler : IRequestHandler<DecideApprovalStepCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public DecideApprovalStepCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(DecideApprovalStepCommand request, CancellationToken ct)
    {
        var approval = await _context.OrderApprovals
            .Include(a => a.PurchaseOrder)
            .FirstOrDefaultAsync(a => a.Id == request.ApprovalId, ct);

        if (approval is null) return Result.Failure("Approval step not found.");
        if (approval.Status != ApprovalStatus.Pending) return Result.Failure("This step has already been decided.");

        // Check that previous steps are approved
        var previousPending = await _context.OrderApprovals
            .AnyAsync(a => a.PurchaseOrderId == approval.PurchaseOrderId
                        && a.StepOrder < approval.StepOrder
                        && a.Status == ApprovalStatus.Pending, ct);
        if (previousPending) return Result.Failure("Previous approval steps must be completed first.");

        approval.Status = request.Decision;
        approval.Comments = request.Comments;
        approval.DecidedAt = DateTime.UtcNow;
        approval.ApproverUserId = _currentUser.UserId;

        if (request.Decision == ApprovalStatus.Rejected)
        {
            approval.PurchaseOrder.Status = OrderStatus.Cancelled;
        }
        else
        {
            // Check if all steps are now approved
            var allApprovals = await _context.OrderApprovals
                .Where(a => a.PurchaseOrderId == approval.PurchaseOrderId)
                .ToListAsync(ct);

            var allApproved = allApprovals.All(a => a.Id == approval.Id || a.Status == ApprovalStatus.Approved);
            if (allApproved)
            {
                approval.PurchaseOrder.Status = OrderStatus.Approved;
            }
            else
            {
                // Notify next approver
                var nextStep = allApprovals
                    .Where(a => a.StepOrder > approval.StepOrder && a.Status == ApprovalStatus.Pending)
                    .OrderBy(a => a.StepOrder)
                    .FirstOrDefault();

                if (nextStep?.ApproverUserId != null)
                {
                    await _notificationService.SendAsync(nextStep.ApproverUserId.Value, approval.PurchaseOrder.TenantId,
                        "Approval Required", $"Order approval required: {nextStep.StepName}",
                        NotificationType.ApprovalRequired, NotificationPriority.High, ct: ct);
                }
            }
        }

        await _context.SaveChangesAsync(ct);
        return Result.Success();
    }
}
