using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations {
    public class VerifyCodeValidator: AbstractValidator<VerifyCodeDto>
    {
        public VerifyCodeValidator()
        {
            RuleFor(i => i.Code).NotEmpty();
            RuleFor(i => i.Code).NotNull();
            RuleFor(i => i.Code).Length(4, 4);
        }
        
    }
}