using System;
using System.Threading;
using System.Threading.Tasks;
using CacheExtension.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CacheExtension.Core;
using CacheExtension.Memory;
using CacheExtension.Redis;

namespace CacheExtension.Core;

public class DefaultCacheService : ICacheService
{
    private readonly ICacheService _inner;
    private readonly ILogger<DefaultCacheService> _logger;

    public DefaultCacheService(
        IOptions<DefaultCacheOptions> options,
        MemoryCacheService memory,
        RedisCacheService redis,
        ILogger<DefaultCacheService> logger)
    {
        _logger = logger;
        var provider = options.Value.DefaultProvider?.Trim() ?? "Memory";
        _logger.LogDebug("Selecting Cache provider: {Provider}", provider);
        _inner = provider.Equals("Redis", StringComparison.OrdinalIgnoreCase) ? (ICacheService)redis : memory;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
        => _inner.SetAsync(key, value, ttl, cancellationToken);

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        => _inner.GetAsync<T>(key, cancellationToken);

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        => _inner.RemoveAsync(key, cancellationToken);

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        => _inner.ExistsAsync(key, cancellationToken);
}
