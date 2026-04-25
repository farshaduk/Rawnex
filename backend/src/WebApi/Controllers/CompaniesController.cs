using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Companies.Commands;
using Rawnex.Application.Features.Companies.DTOs;
using Rawnex.Application.Features.Companies.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class CompaniesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/verify")]
    [Authorize(Policy = "Permission:companies.manage")]
    public async Task<IActionResult> Verify(Guid id, [FromBody] VerifyCompanyCommand command)
    {
        if (id != command.CompanyId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/members")]
    public async Task<IActionResult> AddMember(Guid id, [FromBody] AddCompanyMemberCommand command)
    {
        if (id != command.CompanyId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpDelete("{companyId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid companyId, Guid userId)
    {
        var result = await Mediator.Send(new RemoveCompanyMemberCommand(companyId, userId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("{id:guid}/documents")]
    public async Task<IActionResult> UploadDocument(Guid id, [FromBody] UploadCompanyDocumentCommand command)
    {
        if (id != command.CompanyId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/departments")]
    public async Task<IActionResult> CreateDepartment(Guid id, [FromBody] CreateDepartmentCommand command)
    {
        if (id != command.CompanyId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetCompanyByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new SearchCompaniesQuery(null, null, pageNumber, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyCompanies()
    {
        var result = await Mediator.Send(new GetMyCompaniesQuery());
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
