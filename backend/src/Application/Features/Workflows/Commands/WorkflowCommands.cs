using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Workflows.DTOs;
using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Workflows.Commands;

public record InitiateApprovalWorkflowCommand(
    Guid PurchaseOrderId,
    List<ApprovalStepInput> Steps) : IRequest<Result<ApprovalWorkflowDto>>;

public record ApprovalStepInput(int StepOrder, string StepName, Guid? ApproverUserId);

public record DecideApprovalStepCommand(
    Guid ApprovalId,
    ApprovalStatus Decision,
    string? Comments) : IRequest<Result>;
