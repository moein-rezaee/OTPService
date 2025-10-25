using FluentValidation;

namespace NotificationService.Application.Features.Notifications.Commands.Send;

public class SendSmsCommandValidator : AbstractValidator<SendSmsCommand>
{
    public SendSmsCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Message)
            .NotEmpty()
            .MaximumLength(500);
    }
}
