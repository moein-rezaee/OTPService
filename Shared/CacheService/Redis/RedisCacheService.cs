using Microsoft.Extensions.Options;
using Shared.CacheService.Abstractions;
using StackExchange.Redis;

namespace Shared.CacheService.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly RedisOptions _options;

        public RedisCacheService(IConnectionMultiplexer redis, IOptions<RedisOptions> options)
        {
            _redis = redis;
            _options = options.Value;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(key);
            return value.HasValue ? System.Text.Json.JsonSerializer.Deserialize<T>(value!) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var jsonValue = System.Text.Json.JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, jsonValue, expiry);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync(key);
        }
    }
}