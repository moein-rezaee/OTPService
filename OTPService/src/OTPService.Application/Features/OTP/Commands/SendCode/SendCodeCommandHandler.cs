using System.Threading;
using System.Threading.Tasks;
using Shared.SmsProvider.Abstractions;
using Shared.SmsProvider.Core;
using MediatR;
using OTPService.Domain.Interfaces;

namespace OTPService.Application.Features.OTP.Commands.SendCode;

public class SendCodeCommandHandler : IRequestHandler<SendCodeCommand, Unit>
{
    private readonly IOtpService _otpService;
    private readonly ISmsProvider _smsService;

    public SendCodeCommandHandler(IOtpService otpService, ISmsProvider smsService)
    {
        _otpService = otpService;
        _smsService = smsService;
    }

    public async Task<Unit> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var code = await _otpService.GenerateCodeAsync(request.PhoneNumber);
        await _smsService.SendAsync(new SmsMessage
        {
            Mobile = request.PhoneNumber,
            Text = code
        });
        return Unit.Value;
    }
}