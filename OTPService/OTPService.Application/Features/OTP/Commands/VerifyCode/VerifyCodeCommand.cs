using MediatR;

namespace OTPService.Application.Features.OTP.Commands.VerifyCode;

public record VerifyCodeCommand : IRequest<bool>
{
    public string PhoneNumber { get; init; } = default!;
    public string Code { get; init; } = default!;
}