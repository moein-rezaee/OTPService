using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations {
    public class VerifyCodeValidator: AbstractValidator<VerifyCodeDto>
    {
        public VerifyCodeValidator()
        {
            RuleFor(i => i.Code)
                .NotEmpty()
                .NotNull()
                .Length(4, 4);
            
            RuleFor(i => i.ValidCode)
                .NotEmpty()
                .NotNull()
                .Equal(i => i.Code);
        }
        
    }
}