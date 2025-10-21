using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.SmsProvider.Abstractions;
using Shared.SmsProvider.Core;

namespace Shared.SmsProvider.Farapayamak
{
    public class FarapayamakSmsProvider : ISmsProvider
    {
        private readonly FarapayamakOptions _options;
        private readonly ILogger<FarapayamakSmsProvider> _logger;

        public FarapayamakSmsProvider(IOptions<FarapayamakOptions> options, ILogger<FarapayamakSmsProvider> logger)
        {
            if (options == null || options.Value == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Value;
            _logger = logger;
        }

        public async Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        {
            try
            {
                // TODO: Implement actual Farapayamak API integration
                using (var client = new HttpClient())
                {
                    // Add actual Farapayamak API implementation here
                    throw new NotImplementedException("Actual Farapayamak API integration is not implemented yet");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS via Farapayamak to {Mobile}", message.Mobile);
                return SmsResult.Fail(ex.Message);
            }
        }
    }
}