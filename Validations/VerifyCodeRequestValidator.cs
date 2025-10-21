using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations {
    public class VerifyCodeRequestValidator: AbstractValidator<VerifyCodeRequestDto>
    {
        public VerifyCodeRequestValidator()
        {
            RuleFor(i => i.Mobile)
                .NotEmpty()
                .NotNull()
                .Length(10, 14);

            RuleFor(i => i.Code)
                .NotEmpty()
                .NotNull()
                .Length(4, 4);
        }
        
    }
}

