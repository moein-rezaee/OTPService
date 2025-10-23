using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using CacheExtension.Abstractions;

namespace CacheExtension.Memory;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _cache.TryGetValue(key, out _);
        return Task.FromResult(exists);
    }
}
