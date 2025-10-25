using AuthenticationService.Domain.Interfaces;
using MediatR;

namespace AuthenticationService.Application.Features.Auth.Commands.Send;

public class SendCommandHandler(IOtpClient otpClient) : IRequestHandler<SendCommand, Unit>
{
    private readonly IOtpClient _otpClient = otpClient;
    public async Task<Unit> Handle(SendCommand request, CancellationToken cancellationToken)
    {
        await _otpClient.SendCodeAsync(request.Request.PhoneNumber, cancellationToken);
        return Unit.Value;
    }
}

