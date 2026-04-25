using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Quality.Commands;
using Rawnex.Application.Features.Quality.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class QualityController : BaseApiController
{
    [HttpPost("inspections")]
    public async Task<IActionResult> CreateInspection([FromBody] CreateInspectionCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("inspections/{id:guid}/complete")]
    public async Task<IActionResult> CompleteInspection(Guid id, [FromBody] CompleteInspectionCommand command)
    {
        if (id != command.InspectionId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("inspections/{inspectionId:guid}/reports")]
    public async Task<IActionResult> AddReport(Guid inspectionId, [FromBody] AddQualityReportCommand command)
    {
        if (inspectionId != command.InspectionId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("inspections/{id:guid}")]
    public async Task<IActionResult> GetInspection(Guid id)
    {
        var result = await Mediator.Send(new GetInspectionByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("inspections/order/{orderId:guid}")]
    public async Task<IActionResult> GetOrderInspections(Guid orderId)
    {
        var result = await Mediator.Send(new GetOrderInspectionsQuery(orderId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
