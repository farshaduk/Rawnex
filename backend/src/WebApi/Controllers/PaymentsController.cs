using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rawnex.Application.Features.Payments.Commands;
using Rawnex.Application.Features.Payments.Queries;

namespace Rawnex.WebApi.Controllers;

[Authorize]
public class PaymentsController : BaseApiController
{
    // --- Escrow ---

    [HttpPost("escrow")]
    public async Task<IActionResult> CreateEscrow([FromBody] CreateEscrowAccountCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("escrow/{id:guid}/fund")]
    public async Task<IActionResult> FundEscrow(Guid id, [FromBody] FundEscrowCommand command)
    {
        if (id != command.EscrowAccountId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpPost("escrow/milestones/{milestoneId:guid}/complete")]
    public async Task<IActionResult> CompleteMilestone(Guid milestoneId, [FromBody] CompleteMilestoneCommand command)
    {
        if (milestoneId != command.MilestoneId) return BadRequest(new { error = "Id mismatch." });
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("escrow/order/{orderId:guid}")]
    public async Task<IActionResult> GetEscrowByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetEscrowByOrderQuery(orderId));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    // --- Payments ---

    [HttpPost]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<IActionResult> GetPaymentsByOrder(Guid orderId)
    {
        var result = await Mediator.Send(new GetPaymentsByOrderQuery(orderId));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    // --- Invoices ---

    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        var result = await Mediator.Send(command);
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPost("invoices/{id:guid}/pay")]
    public async Task<IActionResult> MarkInvoicePaid(Guid id)
    {
        var result = await Mediator.Send(new MarkInvoicePaidCommand(id));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok();
    }

    [HttpGet("invoices/{id:guid}")]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        var result = await Mediator.Send(new GetInvoiceByIdQuery(id));
        if (!result.IsSuccess) return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpGet("invoices/company/{companyId:guid}")]
    public async Task<IActionResult> GetCompanyInvoices(Guid companyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await Mediator.Send(new GetCompanyInvoicesQuery(companyId, pageNumber, pageSize));
        if (!result.IsSuccess) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }
}
