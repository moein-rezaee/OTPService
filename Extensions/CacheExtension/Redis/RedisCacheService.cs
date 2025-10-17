using System;
using System.Threading;
using System.Threading.Tasks;
using CacheExtension.Abstractions;

namespace CacheExtension.Redis;

public class RedisCacheService : ICacheService
{
    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        // Intentionally not implemented yet. Wire up a Redis client and set with TTL.
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // Intentionally not implemented yet.
        throw new NotImplementedException();
    }

    public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Intentionally not implemented yet.
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        // Intentionally not implemented yet.
        throw new NotImplementedException();
    }
}
