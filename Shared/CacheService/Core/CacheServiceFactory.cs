using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.CacheService.Abstractions;
using Shared.CacheService.Memory;
using Shared.CacheService.Redis;

using System;
using Shared.CacheService.Core;

namespace Shared.CacheService.Core
{
    public class CacheServiceFactory : ICacheServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DefaultCacheOptions _options;

        public CacheServiceFactory(IServiceProvider serviceProvider, IOptions<DefaultCacheOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public ICacheService Create(CacheProviderKind kind)
        {
            return kind switch
            {
                CacheProviderKind.Memory => ActivatorUtilities.CreateInstance<MemoryCacheService>(_serviceProvider),
                CacheProviderKind.Redis => ActivatorUtilities.CreateInstance<RedisCacheService>(_serviceProvider),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported cache provider")
            };
        }
    }
}