using Microsoft.AspNetCore.Mvc;
using MediatR;
using NotificationService.Application.Features.Notifications.Commands.Send;

namespace NotificationService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("Send")]
    public async Task<IActionResult> Send([FromBody] SendSmsCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok();
    }
}
