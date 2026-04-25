using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Auctions.Commands;
using Rawnex.Application.Features.Auctions.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class AuctionsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/bids")]
    public async Task<IActionResult> PlaceBid(Guid id, [FromBody] PlaceBidCommand command)
    {
        if (id != command.AuctionId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        var result = await Mediator.Send(new StartAuctionCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/end")]
    public async Task<IActionResult> End(Guid id)
    {
        var result = await Mediator.Send(new EndAuctionCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await Mediator.Send(new CancelAuctionCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetAuctionByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchAuctionsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
