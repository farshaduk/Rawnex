using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Workflows.Commands;
using Rawnex.Application.Features.Workflows.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class WorkflowsController : BaseApiController
{
    [HttpPost("orders/{orderId:guid}/approvals")]
    public async Task<IActionResult> InitiateApproval(Guid orderId, [FromBody] InitiateApprovalWorkflowCommand command)
    {
        if (orderId != command.PurchaseOrderId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("approvals/{approvalId:guid}/decide")]
    public async Task<IActionResult> DecideStep(Guid approvalId, [FromBody] DecideApprovalStepCommand command)
    {
        if (approvalId != command.ApprovalId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("orders/{orderId:guid}/approvals")]
    public async Task<IActionResult> GetOrderWorkflow(Guid orderId)
    {
        var result = await Mediator.Send(new GetOrderApprovalWorkflowQuery(orderId));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }
}
