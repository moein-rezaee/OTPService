using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CacheExtension.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace CacheExtension.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _muxer;
    private readonly RedisOptions _options;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer muxer, IOptions<RedisOptions> options, ILogger<RedisCacheService> logger)
    {
        _muxer = muxer;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        var db = _muxer.GetDatabase(_options.DefaultDatabase ?? -1);
        var payload = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, payload, ttl).ConfigureAwait(false);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var db = _muxer.GetDatabase(_options.DefaultDatabase ?? -1);
        var data = await db.StringGetAsync(key).ConfigureAwait(false);
        if (data.IsNullOrEmpty) return default;
        try
        {
            return JsonSerializer.Deserialize<T>(data!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deserialize cached value for {Key}", key);
            return default;
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var db = _muxer.GetDatabase(_options.DefaultDatabase ?? -1);
        return await db.KeyDeleteAsync(key).ConfigureAwait(false);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var db = _muxer.GetDatabase(_options.DefaultDatabase ?? -1);
        return await db.KeyExistsAsync(key).ConfigureAwait(false);
    }
}
