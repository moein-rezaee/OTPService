using MediatR;

namespace NotificationService.Application.Features.Notifications.Commands.Send;

public record SendSmsCommand : IRequest<Unit>
{
    public string PhoneNumber { get; init; } = default!;
    public string Message { get; init; } = default!;
}
