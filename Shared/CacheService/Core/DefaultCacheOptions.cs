namespace Shared.CacheService.Core
{
    public class DefaultCacheOptions
    {
        public CacheProviderKind DefaultProvider { get; set; } = CacheProviderKind.Memory;
        public int DefaultExpirationMinutes { get; set; } = 5;
    }
}