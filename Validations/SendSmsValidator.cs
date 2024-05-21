using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations
{
    public class SendSmsValidator : AbstractValidator<SendSmsDto>
    {
        public SendSmsValidator()
        {
            RuleFor(i => i.Receivers)
                .NotEmpty()
                .NotNull()
                .MinimumLength(10);

            RuleFor(i => i.Message)
                .NotEmpty()
                .NotNull()
                .Length(3, 60);
        }

    }
}