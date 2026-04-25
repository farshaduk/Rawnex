using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Rawnex.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected string? IpAddress => HttpContext.Connection.RemoteIpAddress?.ToString();
    protected string? UserAgentHeader => Request.Headers.UserAgent.ToString();
}
