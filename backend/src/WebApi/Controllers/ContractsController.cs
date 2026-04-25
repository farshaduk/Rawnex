using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Contracts.Commands;
using Rawnex.Application.Features.Contracts.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class ContractsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateContractCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/sign")]
    public async Task<IActionResult> Sign(Guid id, [FromBody] SignContractCommand command)
    {
        if (id != command.ContractId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/terminate")]
    public async Task<IActionResult> Terminate(Guid id, [FromBody] TerminateContractCommand command)
    {
        if (id != command.ContractId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetContractByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("company/{companyId:guid}")]
    public async Task<IActionResult> GetCompanyContracts(Guid companyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetCompanyContractsQuery(companyId, pageNumber, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
