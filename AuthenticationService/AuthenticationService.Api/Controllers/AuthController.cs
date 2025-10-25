using AuthenticationService.Application.Features.Auth.Commands.Logout;
using AuthenticationService.Application.Features.Auth.Commands.Refresh;
using AuthenticationService.Application.Features.Auth.Commands.Send;
using AuthenticationService.Application.Features.Auth.Commands.Verify;
using AuthenticationService.Application.Features.Auth.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] SendCodeRequest request, CancellationToken ct)
    {
        await _mediator.Send(new SendCommand(request), ct);
        return Ok();
    }

    [HttpPost("Verify")]
    public async Task<ActionResult<TokenResponse>> Verify([FromBody] VerifyCodeRequest request, CancellationToken ct)
    {
        var (user, access, refresh) = await _mediator.Send(new VerifyCommand(request), ct);
        return Ok(new TokenResponse(access, refresh));
    }

    [HttpPost("Refresh")]
    public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var (user, access, refresh) = await _mediator.Send(new RefreshCommand(request), ct);
        return Ok(new TokenResponse(access, refresh));
    }

    [HttpPost("Logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        await _mediator.Send(new LogoutCommand(request), ct);
        return Ok();
    }
}

