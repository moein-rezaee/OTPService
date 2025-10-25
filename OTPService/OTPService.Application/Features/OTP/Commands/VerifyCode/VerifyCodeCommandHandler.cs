using MediatR;
using OTPService.Domain.Interfaces;

namespace OTPService.Application.Features.OTP.Commands.VerifyCode;

public class VerifyCodeCommandHandler : IRequestHandler<VerifyCodeCommand, bool>
{
    private readonly IOtpService _otpService;

    public VerifyCodeCommandHandler(IOtpService otpService)
    {
        _otpService = otpService;
    }

    public async Task<bool> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        return await _otpService.ValidateCodeAsync(request.PhoneNumber, request.Code);
    }
}
