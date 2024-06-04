using CustomResponse.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace OTPService.Services
{
    public class RedisCacheService(IDistributedCache cache)
    {
        private readonly IDistributedCache _cache = cache;

    }
}