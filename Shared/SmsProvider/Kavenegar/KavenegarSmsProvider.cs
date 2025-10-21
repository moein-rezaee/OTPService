using Kavenegar;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SmsProvider.Core;
using Shared.SmsProvider.Abstractions;

namespace Shared.SmsProvider.Kavenegar
{
    public class KavenegarSmsProvider : ISmsProvider
    {
        private readonly KavenegarOptions _options;
        private readonly ILogger<KavenegarSmsProvider> _logger;

        public KavenegarSmsProvider(IOptions<KavenegarOptions> options, ILogger<KavenegarSmsProvider> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Kavenegar provider invoked for {Mobile}", message.Mobile);

            if (string.IsNullOrWhiteSpace(_options.ApiKey))
                return SmsResult.Fail("Kavenegar ApiKey is not configured");
            if (string.IsNullOrWhiteSpace(_options.Sender))
                return SmsResult.Fail("Kavenegar Sender is not configured");
            if (string.IsNullOrWhiteSpace(message.Mobile))
                return SmsResult.Fail("Mobile is required");
            if (string.IsNullOrWhiteSpace(message.Text))
                return SmsResult.Fail("Message is required");

            try
            {
                var api = new global::Kavenegar.KavenegarApi(_options.ApiKey);
                var sendResult = await Task.Run(() => api.Send(_options.Sender, message.Mobile, message.Text), cancellationToken);
                return SmsResult.Ok(sendResult.Status.ToString());
            }
            catch (Exception ex)
            {
                return SmsResult.Fail($"Kavenegar error: {ex.Message}");
            }
        }
    }
}