using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Disputes.Commands;
using Rawnex.Application.Features.Disputes.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class DisputesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> File([FromBody] FileDisputeCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/evidence")]
    public async Task<IActionResult> AddEvidence(Guid id, [FromBody] AddDisputeEvidenceCommand command)
    {
        if (id != command.DisputeId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/resolve")]
    [Authorize(Policy = "Permission:disputes.manage")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveDisputeCommand command)
    {
        if (id != command.DisputeId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetDisputeByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetOrderDisputesQuery(orderId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
