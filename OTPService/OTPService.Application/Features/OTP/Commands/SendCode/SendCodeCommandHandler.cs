using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OTPService.Domain.Interfaces;

namespace OTPService.Application.Features.OTP.Commands.SendCode;

public class SendCodeCommandHandler : IRequestHandler<SendCodeCommand, Unit>
{
    private readonly IOtpService _otpService;

    public SendCodeCommandHandler(IOtpService otpService)
    {
        _otpService = otpService;
    }

    public async Task<Unit> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        await _otpService.SendCodeAsync(request.PhoneNumber);
        return Unit.Value;
    }
}
