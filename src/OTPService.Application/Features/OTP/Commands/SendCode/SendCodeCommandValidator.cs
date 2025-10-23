using FluentValidation;

namespace OTPService.Application.Features.OTP.Commands.SendCode;

public class SendCodeCommandValidator : AbstractValidator<SendCodeCommand>
{
    public SendCodeCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره موبایل وارد شده معتبر نمی‌باشد");
    }
}