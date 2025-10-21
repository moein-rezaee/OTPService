using Shared.CacheService.Core;

namespace Shared.CacheService.Abstractions
{
    public interface ICacheServiceFactory
    {
        ICacheService Create(CacheProviderKind kind);
    }
}