using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmsExtension.Core;
using SmsExtension.Abstractions;
using Farapayamak;

namespace SmsExtension.Provider.Farapayamak;

public class FarapayamakSmsProvider : ISmsProvider
{
    private readonly FarapayamakOptions _options;
    private readonly ILogger<FarapayamakSmsProvider> _logger;

    public FarapayamakSmsProvider(IOptions<FarapayamakOptions> options, ILogger<FarapayamakSmsProvider> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Farapayamak provider invoked for {Mobile}", message.Mobile);

        if (string.IsNullOrWhiteSpace(_options.Username))
            return SmsResult.Fail("Farapayamak Username is not configured");
        if (string.IsNullOrWhiteSpace(_options.Password))
            return SmsResult.Fail("Farapayamak Password is not configured");
        if (string.IsNullOrWhiteSpace(_options.From))
            return SmsResult.Fail("Farapayamak From is not configured");
        if (string.IsNullOrWhiteSpace(message.Mobile))
            return SmsResult.Fail("Mobile is required");
        if (string.IsNullOrWhiteSpace(message.Text))
            return SmsResult.Fail("Message is required");

        try
        {
            var client = new RestClient(_options.Username, _options.Password);
            var response = await Task.Run(() => client.SendSMS(message.Mobile, _options.From, message.Text), cancellationToken);
            var success = response is not null && response.ToString()!.Length > 0;
            return success ? SmsResult.Ok() : SmsResult.Fail("Farapayamak send failed");
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Farapayamak send error for {Mobile}", message.Mobile);
            return SmsResult.Fail("Farapayamak error");
        }
    }
}
