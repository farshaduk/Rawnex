using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Admin.Commands;
using Rawnex.Application.Features.Admin.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize(Policy = "Permission:admin.manage")]
public class AdminController : BaseApiController
{
    // --- Commission Rules ---

    [HttpPost("commission-rules")]
    public async Task<IActionResult> CreateCommissionRule([FromBody] CreateCommissionRuleCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("commission-rules/{id:guid}")]
    public async Task<IActionResult> UpdateCommissionRule(Guid id, [FromBody] UpdateCommissionRuleCommand command)
    {
        if (id != command.Id) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpDelete("commission-rules/{id:guid}")]
    public async Task<IActionResult> DeleteCommissionRule(Guid id)
    {
        var result = await Mediator.Send(new DeleteCommissionRuleCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("commission-rules")]
    public async Task<IActionResult> GetCommissionRules()
    {
        var result = await Mediator.Send(new GetCommissionRulesQuery());
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    // --- Feature Flags ---

    [HttpPut("feature-flags")]
    public async Task<IActionResult> UpsertFeatureFlag([FromBody] UpsertFeatureFlagCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete("feature-flags/{key}")]
    public async Task<IActionResult> DeleteFeatureFlag(string key)
    {
        var result = await Mediator.Send(new DeleteFeatureFlagCommand(key));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("feature-flags")]
    public async Task<IActionResult> GetFeatureFlags()
    {
        var result = await Mediator.Send(new GetFeatureFlagsQuery());
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    // --- Tenants ---

    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenants([FromQuery] GetTenantsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("tenants/{id:guid}")]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var result = await Mediator.Send(new GetTenantByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPatch("tenants/{id:guid}/status")]
    public async Task<IActionResult> UpdateTenantStatus(Guid id, [FromBody] UpdateTenantStatusCommand command)
    {
        if (id != command.TenantId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPatch("tenants/{id:guid}/plan")]
    public async Task<IActionResult> UpdateTenantPlan(Guid id, [FromBody] UpdateTenantPlanCommand command)
    {
        if (id != command.TenantId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    // --- Platform Billing ---

    [HttpGet("billings")]
    public async Task<IActionResult> GetBillings([FromQuery] GetPlatformBillingsQuery query)
    {
        var result = await Mediator.Send(query);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("billings/{id:guid}/pay")]
    public async Task<IActionResult> MarkBillingPaid(Guid id)
    {
        var result = await Mediator.Send(new MarkBillingPaidCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }
}
