using System.Collections.Generic;
using Microsoft.Extensions.Options;
using CacheExtension.Abstractions;
using CacheExtension.Memory;
using CacheExtension.Redis;

namespace CacheExtension.Core;

public class CacheServiceFactory : ICacheServiceFactory
{
    private readonly DefaultCacheOptions _options;
    private readonly MemoryCacheService _memory;
    private readonly RedisCacheService _redis;

    public CacheServiceFactory(
        IOptions<DefaultCacheOptions> options,
        MemoryCacheService memory,
        RedisCacheService redis)
    {
        _options = options.Value;
        _memory = memory;
        _redis = redis;
    }

    public ICacheService GetDefault() => MapNameToKind(_options.DefaultProvider) == CacheProviderKind.Redis ? _redis : _memory;

    public ICacheService Get(CacheProviderKind kind) => kind == CacheProviderKind.Redis ? _redis : _memory;

    public IEnumerable<ICacheService> GetAll() => new ICacheService[] { _redis, _memory };

    private static CacheProviderKind MapNameToKind(string? name)
        => (name ?? string.Empty).Trim().Equals("Redis", System.StringComparison.OrdinalIgnoreCase)
            ? CacheProviderKind.Redis
            : CacheProviderKind.Memory;
}
