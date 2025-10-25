using MediatR;
using SmsExtension.Abstractions;
using SmsExtension.Core;

namespace NotificationService.Application.Features.Notifications.Commands.Send;

public class SendSmsCommandHandler(ISmsProvider smsProvider) : IRequestHandler<SendSmsCommand, Unit>
{
    private readonly ISmsProvider _smsProvider = smsProvider;

    public async Task<Unit> Handle(SendSmsCommand request, CancellationToken cancellationToken)
    {
        await _smsProvider.SendAsync(new SmsMessage
        {
            Mobile = request.PhoneNumber,
            Text = request.Message
        }, cancellationToken);

        return Unit.Value;
    }
}
