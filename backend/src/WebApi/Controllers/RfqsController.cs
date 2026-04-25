using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Rfqs.Commands;
using Rawnex.Application.Features.Rfqs.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class RfqsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRfqCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/responses")]
    public async Task<IActionResult> SubmitResponse(Guid id, [FromBody] SubmitRfqResponseCommand command)
    {
        if (id != command.RfqId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/award")]
    public async Task<IActionResult> Award(Guid id, [FromBody] AwardRfqCommand command)
    {
        if (id != command.RfqId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await Mediator.Send(new CancelRfqCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetRfqByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchRfqsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("my-responses")]
    public async Task<IActionResult> GetMyResponses([FromQuery] Guid sellerCompanyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetMyRfqResponsesQuery(sellerCompanyId, pageNumber, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
