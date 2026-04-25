using MediatR;
using Rawnex.Application.Common.Models;
using Rawnex.Application.Features.Workflows.DTOs;

namespace Rawnex.Application.Features.Workflows.Queries;

public record GetOrderApprovalWorkflowQuery(Guid PurchaseOrderId) : IRequest<Result<ApprovalWorkflowDto>>;
