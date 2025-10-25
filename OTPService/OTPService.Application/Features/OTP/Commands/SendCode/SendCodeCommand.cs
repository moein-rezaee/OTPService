using MediatR;

namespace OTPService.Application.Features.OTP.Commands.SendCode;

public record SendCodeCommand : IRequest<Unit>
{
    public string PhoneNumber { get; init; } = default!;
}
