using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Webhooks.Commands;
using Rawnex.Application.Features.Webhooks.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class WebhooksController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWebhookSubscriptionCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWebhookSubscriptionCommand command)
    {
        if (id != command.SubscriptionId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await Mediator.Send(new DeleteWebhookSubscriptionCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/test")]
    public async Task<IActionResult> Test(Guid id)
    {
        var result = await Mediator.Send(new TestWebhookCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetMySubscriptions([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetMyWebhookSubscriptionsQuery(page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/deliveries")]
    public async Task<IActionResult> GetDeliveries(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetWebhookDeliveriesQuery(id, page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
