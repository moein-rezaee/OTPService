using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations
{
    public class SendCodeValidator : AbstractValidator<SendCodeDto>
    {
        public SendCodeValidator()
        {
            RuleFor(d => d.Mobile).NotEmpty()
                              .NotNull()
                              .Matches(@"^(\+98|0)?9\d{9}$")
                              .WithMessage("شماره وارد شده معتبر نمی باشد!");

            RuleFor(i => i.OrganizationId).NotEmpty().NotNull();
        }
    }
}