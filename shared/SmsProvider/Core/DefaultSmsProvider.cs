using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmsExtension.Abstractions;
using SmsExtension.Core;
using SmsExtension.Provider.Kavenegar;
using SmsExtension.Provider.Farapayamak;

namespace SmsExtension.Core;

public class DefaultSmsProvider : ISmsProvider
{
    private readonly ISmsProvider _inner;
    private readonly ILogger<DefaultSmsProvider> _logger;

    public DefaultSmsProvider(
        IOptions<DefaultProviderOptions> options,
        KavenegarSmsProvider kavenegar,
        FarapayamakSmsProvider farapayamak,
        ILogger<DefaultSmsProvider> logger)
    {
        _logger = logger;
        var provider = options.Value.DefaultProvider?.Trim() ?? "Farapayamak";
        _logger.LogDebug("Selecting SMS provider: {Provider}", provider);

        if (provider.Equals("Kavenegar", StringComparison.OrdinalIgnoreCase))
        {
            _inner = kavenegar;
        }
        else
        {
            _inner = farapayamak;
        }
    }

    public Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
        => _inner.SendAsync(message, cancellationToken);
}
