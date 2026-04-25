using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Listings.Commands;
using Rawnex.Application.Features.Listings.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class ListingsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingCommand command)
    {
        if (id != command.ListingId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromBody] ChangeListingStatusCommand command)
    {
        if (id != command.ListingId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteListingCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetListingByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchListingsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
