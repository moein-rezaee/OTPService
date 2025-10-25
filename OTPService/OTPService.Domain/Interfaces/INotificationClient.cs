namespace OTPService.Domain.Interfaces;

public interface INotificationClient
{
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}

