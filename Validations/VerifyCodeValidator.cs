using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations
{
    public class VerifyCodeValidator : AbstractValidator<VerifyCodeDto>
    {
        public VerifyCodeValidator()
        {
            RuleFor(d => d.Mobile).NotEmpty()
                                 .NotNull()
                                 .Matches(@"^(\+98|0)?9\d{9}$")
                                 .WithMessage("شماره وارد شده معتبر نمی باشد!");

            RuleFor(i => i.Code)
                           .NotEmpty()
                           .NotNull()
                           .Length(4, 4);

            RuleFor(i => i.Code)
                .NotEmpty()
                .NotNull()
                .Length(4, 4);

            RuleFor(i => i.OrganizationId)
                .NotEmpty()
                .NotNull();
        }

    }
}