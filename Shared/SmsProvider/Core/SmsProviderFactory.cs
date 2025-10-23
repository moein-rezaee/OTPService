using System.Collections.Generic;
using Microsoft.Extensions.Options;
using SmsExtension.Abstractions;
using SmsExtension.Provider.Kavenegar;
using SmsExtension.Provider.Farapayamak;

namespace SmsExtension.Core;

public class SmsProviderFactory : ISmsProviderFactory
{
    private readonly DefaultProviderOptions _options;
    private readonly KavenegarSmsProvider _kavenegar;
    private readonly FarapayamakSmsProvider _farapayamak;

    public SmsProviderFactory(
        IOptions<DefaultProviderOptions> options,
        KavenegarSmsProvider kavenegar,
        FarapayamakSmsProvider farapayamak)
    {
        _options = options.Value;
        _kavenegar = kavenegar;
        _farapayamak = farapayamak;
    }

    public ISmsProvider GetDefault() => MapNameToKind(_options.DefaultProvider) switch
    {
        SmsProviderKind.Kavenegar => _kavenegar,
        _ => _farapayamak
    };

    public ISmsProvider Get(SmsProviderKind kind) => kind switch
    {
        SmsProviderKind.Kavenegar => _kavenegar,
        SmsProviderKind.Farapayamak => _farapayamak,
        _ => _farapayamak
    };

    public IEnumerable<ISmsProvider> GetAll() => new ISmsProvider[] { _kavenegar, _farapayamak };

    private static SmsProviderKind MapNameToKind(string? name)
    {
        return (name ?? string.Empty).Trim().Equals("Kavenegar", System.StringComparison.OrdinalIgnoreCase)
            ? SmsProviderKind.Kavenegar
            : SmsProviderKind.Farapayamak;
    }
}
