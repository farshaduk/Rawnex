using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Verification.Commands;
using Rawnex.Application.Features.Verification.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class VerificationController : BaseApiController
{
    // KYC
    [HttpPost("kyc")]
    public async Task<IActionResult> SubmitKyc([FromBody] SubmitKycCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("kyc/{id:guid}/review")]
    public async Task<IActionResult> ReviewKyc(Guid id, [FromBody] ReviewKycCommand command)
    {
        if (id != command.KycId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("kyc/me")]
    public async Task<IActionResult> GetMyKycStatus()
    {
        var result = await Mediator.Send(new GetMyKycStatusQuery());
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("kyc/pending")]
    public async Task<IActionResult> GetPendingKycList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPendingKycListQuery(page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    // KYB
    [HttpPost("kyb")]
    public async Task<IActionResult> SubmitKyb([FromBody] SubmitKybCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("kyb/{id:guid}/review")]
    public async Task<IActionResult> ReviewKyb(Guid id, [FromBody] ReviewKybCommand command)
    {
        if (id != command.KybId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("kyb/me")]
    public async Task<IActionResult> GetMyKybStatus([FromQuery] Guid companyId)
    {
        var result = await Mediator.Send(new GetMyKybStatusQuery(companyId));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("kyb/pending")]
    public async Task<IActionResult> GetPendingKybList([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetPendingKybListQuery(page, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
