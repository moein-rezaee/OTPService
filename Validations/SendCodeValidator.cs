using FluentValidation;
using OTPService.DTOs;

namespace OTPService.Validations {
    public class SendCodeValidator: AbstractValidator<SendCodeDto>
    {
        public SendCodeValidator()
        {
            RuleFor(i => i.Mobile).NotEmpty();
            RuleFor(i => i.Mobile).NotNull();
            RuleFor(i => i.Mobile).Length(10, 14);
        }
        
    }
}