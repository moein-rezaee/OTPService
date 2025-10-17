using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmsExtension.Core;
using SmsExtension.Provider.Farapayamak;
using SmsExtension.Provider.Kavenegar;

namespace SmsExtension;

public class DefaultSmsProvider : ISmsProvider
{
    private readonly IOptions<DefaultProviderOptions> _options;
    private readonly KavenegarSmsProvider _kavenegar;
    private readonly FarapayamakSmsProvider _farapayamak;
    private readonly ILogger<DefaultSmsProvider> _logger;

    public DefaultSmsProvider(
        IOptions<DefaultProviderOptions> options,
        KavenegarSmsProvider kavenegar,
        FarapayamakSmsProvider farapayamak,
        ILogger<DefaultSmsProvider> logger)
    {
        _options = options;
        _kavenegar = kavenegar;
        _farapayamak = farapayamak;
        _logger = logger;
    }

    public Task<SmsResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default)
    {
        var provider = _options.Value.DefaultProvider ?? "Kavenegar";
        _logger.LogDebug("Routing SMS to provider: {Provider}", provider);
        if (string.Equals(provider, "Farapayamak", StringComparison.OrdinalIgnoreCase))
            return _farapayamak.SendAsync(message, cancellationToken);
        return _kavenegar.SendAsync(message, cancellationToken);
    }
}

