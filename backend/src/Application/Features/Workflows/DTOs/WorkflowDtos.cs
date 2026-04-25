using Rawnex.Domain.Enums;

namespace Rawnex.Application.Features.Workflows.DTOs;

public record ApprovalWorkflowDto(
    Guid OrderId,
    IReadOnlyList<ApprovalStepDto> Steps,
    bool IsFullyApproved);

public record ApprovalStepDto(
    Guid Id,
    int StepOrder,
    string StepName,
    Guid? ApproverUserId,
    string? ApproverName,
    ApprovalStatus Status,
    string? Comments,
    DateTime? DecidedAt);
