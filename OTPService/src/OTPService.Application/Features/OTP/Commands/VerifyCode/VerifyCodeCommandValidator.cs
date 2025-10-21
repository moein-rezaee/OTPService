using FluentValidation;

namespace OTPService.Application.Features.OTP.Commands.VerifyCode;

public class VerifyCodeCommandValidator : AbstractValidator<VerifyCodeCommand>
{
    public VerifyCodeCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره موبایل وارد شده معتبر نمی‌باشد");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(5)
            .Matches(@"^\d+$")
            .WithMessage("کد وارد شده معتبر نمی‌باشد");
    }
}