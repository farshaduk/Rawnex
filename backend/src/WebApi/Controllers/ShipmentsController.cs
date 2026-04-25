using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Shipments.Commands;
using Rawnex.Application.Features.Shipments.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class ShipmentsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShipmentCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateShipmentStatusCommand command)
    {
        if (id != command.ShipmentId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/batches")]
    public async Task<IActionResult> AddBatch(Guid id, [FromBody] AddBatchToShipmentCommand command)
    {
        if (id != command.ShipmentId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("freight-quotes")]
    public async Task<IActionResult> RequestFreightQuote([FromBody] RequestFreightQuoteCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("freight-quotes/{id:guid}/select")]
    public async Task<IActionResult> SelectFreightQuote(Guid id)
    {
        var result = await Mediator.Send(new SelectFreightQuoteCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetShipmentByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetOrderShipmentsQuery(orderId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("freight-quotes/shipment/{shipmentId:guid}")]
    public async Task<IActionResult> GetFreightQuotes(Guid shipmentId)
    {
        var result = await Mediator.Send(new GetFreightQuotesQuery(shipmentId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
