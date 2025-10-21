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

    /// <summary>
    /// Sends an OTP code to the specified phone number
    /// </summary>
    /// <param name="command">The command containing the phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response when the code is sent</returns>
    /// <response code="200">OTP code was sent successfully</response>
    /// <response code="400">Invalid phone number format</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("send")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendCode([FromBody] SendCodeCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Verifies an OTP code for a phone number
    /// </summary>
    /// <param name="command">The command containing the phone number and code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success response with verification result</returns>
    /// <response code="200">Code verification completed</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
