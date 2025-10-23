using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CacheExtension.Abstractions;
using CacheExtension.Core;
using CacheExtension.Memory;
using CacheExtension.Redis;

namespace CacheExtension;

public static class DependencyInjection
{
    public static IServiceCollection AddCacheExtension(this IServiceCollection services, IConfiguration configuration)
    {
        // Memory cache is always available
        services.AddMemoryCache();

        // Bind default options
        var defaultOptions = new DefaultCacheOptions();
        configuration.GetSection("Cache").Bind(defaultOptions);
        services.Configure<DefaultCacheOptions>(opt =>
        {
            opt.DefaultProvider = configuration.GetValue<string>("Cache:DefaultProvider") ?? defaultOptions.DefaultProvider;
        });

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

        // Redis configuration (optional)
        var redisSection = configuration.GetSection("Cache:Providers:Redis");
        var redisOptions = new RedisOptions();
        redisSection.Bind(redisOptions);
        var redisConfigured = redisSection.Exists();

        if (redisConfigured)
        {
            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                var cfg = new ConfigurationOptions();
                cfg.EndPoints.Add(ExpandEnv(redisSection["Host"]) ?? redisOptions.Host, 
                                  int.TryParse(ExpandEnv(redisSection["Port"]), out var p) ? p : redisOptions.Port);
                cfg.User = ExpandEnv(redisSection["Username"]);
                cfg.Password = ExpandEnv(redisSection["Password"]);
                return ConnectionMultiplexer.Connect(cfg);
            });

            services.Configure<RedisOptions>(opt =>
            {
                opt.Host = ExpandEnv(redisSection["Host"]) ?? redisOptions.Host;
                opt.Port = int.TryParse(ExpandEnv(redisSection["Port"]), out var p) ? p : redisOptions.Port;
                opt.Username = ExpandEnv(redisSection["Username"]);
                opt.Password = ExpandEnv(redisSection["Password"]);
                opt.InstanceName = ExpandEnv(redisSection["InstanceName"]);
            });
        }

        // Concrete services and factory
        services.AddScoped<MemoryCacheService>();
        if (redisConfigured)
            services.AddScoped<RedisCacheService>();

        services.AddScoped<ICacheServiceFactory, CacheServiceFactory>();

        // Default ICacheService binding
        var defaultProvider = (configuration.GetValue<string>("Cache:DefaultProvider") ?? defaultOptions.DefaultProvider).Trim();
        if (string.Equals(defaultProvider, "Redis", StringComparison.OrdinalIgnoreCase) && redisConfigured)
            services.AddScoped<ICacheService, RedisCacheService>();
        else
            services.AddScoped<ICacheService, MemoryCacheService>();

        return services;
    }
}

