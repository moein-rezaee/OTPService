using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OTPService.Domain.Interfaces;

namespace OTPService.Infrastructure.Notification;

public class NotificationHttpClient : INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationHttpClient> _logger;

    public NotificationHttpClient(HttpClient httpClient, ILogger<NotificationHttpClient> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;

        var baseUrl = configuration["Notification:BaseUrl"] ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending SMS via Notification service to {Phone}", phoneNumber);

        var payload = new { PhoneNumber = phoneNumber, Message = message };
        using var response = await _httpClient.PostAsJsonAsync("api/Notifications/Send", payload, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
