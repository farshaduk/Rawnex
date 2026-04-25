using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Ratings.Commands;
using Rawnex.Application.Features.Ratings.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class RatingsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitRatingCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/respond")]
    public async Task<IActionResult> Respond(Guid id, [FromBody] RespondToRatingCommand command)
    {
        if (id != command.RatingId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("company/{companyId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompanyRatings(Guid companyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetCompanyRatingsQuery(companyId, pageNumber, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetOrderRating(Guid orderId, [FromQuery] Guid reviewerCompanyId)
    {
        var result = await Mediator.Send(new GetOrderRatingQuery(orderId, reviewerCompanyId));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }
}
