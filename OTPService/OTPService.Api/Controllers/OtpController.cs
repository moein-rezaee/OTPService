using Microsoft.AspNetCore.Mvc;
using MediatR;
using OTPService.Application.Features.OTP.Commands.SendCode;
using OTPService.Application.Features.OTP.Commands.VerifyCode;

namespace OTPService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OtpController : ControllerBase
{
    private readonly IMediator _mediator;

    public OtpController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Send")]
    public async Task<IActionResult> SendCode([FromBody] SendCodeCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }

    [HttpPost("Verify")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
