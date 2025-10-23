using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmsExtension.Abstractions;
using SmsExtension.Core;
using SmsExtension.Provider.Farapayamak;
using SmsExtension.Provider.Kavenegar;

namespace SmsExtension;

public static class DependencyInjection
{
    public static IServiceCollection AddSmsExtension(this IServiceCollection services, IConfiguration configuration)
    {
        // Helper to expand ${ENV} placeholders
        static string ExpandEnv(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            value = value.Trim();
            if (value.StartsWith("${") && value.EndsWith("}"))
            {
                var key = value.Substring(2, value.Length - 3);
                var env = Environment.GetEnvironmentVariable(key);
                return string.IsNullOrWhiteSpace(env) ? value : env!;
            }
            return value;
        }

        // Read provider sections
        var kav = configuration.GetSection("Sms:Providers:Kavenegar");
        var fpp = configuration.GetSection("Sms:Providers:Farapayamak");

        var kavApiKey = ExpandEnv(kav["ApiKey"]);
        var kavSender = ExpandEnv(kav["Sender"]);
        var kavenegarConfigured = !string.IsNullOrWhiteSpace(kavApiKey) && !string.IsNullOrWhiteSpace(kavSender);

        var fUser = ExpandEnv(fpp["Username"]);
        var fPass = ExpandEnv(fpp["Password"]);
        var fFrom = ExpandEnv(fpp["From"]) ?? ExpandEnv(fpp["Sender"]);
        var farapayamakConfigured = !string.IsNullOrWhiteSpace(fUser) && !string.IsNullOrWhiteSpace(fPass) && !string.IsNullOrWhiteSpace(fFrom);

        if (kavenegarConfigured)
        {
            services.Configure<KavenegarOptions>(opt =>
            {
                opt.ApiKey = kavApiKey;
                opt.Sender = kavSender;
            });
        }

        if (farapayamakConfigured)
        {
            services.Configure<FarapayamakOptions>(opt =>
            {
                opt.Username = fUser;
                opt.Password = fPass;
                opt.From = fFrom ?? string.Empty;
            });
        }

        services.Configure<DefaultProviderOptions>(o =>
        {
            o.DefaultProvider = configuration.GetValue<string>("Sms:DefaultProvider")
                ?? configuration.GetValue<string>("Sms:Provider")
                ?? "Farapayamak";
        });

        // Concrete providers + factory + default
        services.AddScoped<KavenegarSmsProvider>();
        services.AddScoped<FarapayamakSmsProvider>();
        services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
        services.AddScoped<ISmsProvider, DefaultSmsProvider>();

        return services;
    }
}

