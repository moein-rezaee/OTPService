# CacheExtension (Wrapper کش)

هدف
- یک API یکسان برای کش با پیاده‌سازی‌های مختلف (Redis/Memory).
- انتخاب پیش‌فرض از کانفیگ و امکان تغییر در زمان کدنویسی با Factory.

ساختار پوشه‌ها
- Abstractions
  - `ICacheService`: قرارداد کش (Set/Get/Remove/Exists) همه async با TTL.
  - `ICacheServiceFactory`: گرفتن کلاینت پیش‌فرض/خاص؛ `GetDefault()`, `Get(CacheProviderKind)`, `GetAll()`.
- Core
  - `DefaultCacheOptions`: تعیین `DefaultProvider` (Redis/Memory).
  - `DefaultCacheService`: Router که یک‌بار بر اساس کانفیگ انتخاب و سپس دلیگیت می‌کند.
  - `CacheServiceFactory`: ساخت/بازگردانی کلاینت‌ها بر اساس enum `CacheProviderKind`.
  - `CacheProviderKind`: `Redis`, `Memory`.
- Redis
  - `RedisOptions { Host, Port, Username, Password, DefaultDatabase }`
  - `RedisCacheService` (StackExchange.Redis)
- Memory
  - `MemoryCacheService` (Microsoft.Extensions.Caching.Memory)

کانفیگ (appsettings)
```
"Cache": {
  "DefaultProvider": "Redis",
  "Providers": {
    "Redis": {
      "Host": "localhost",
      "Port": 6379,
      "Username": "",
      "Password": "",
      "DefaultDatabase": 0
    }
  }
}
```

ثبت در DI (نمونه)
```
services.AddMemoryCache();
services.Configure<DefaultCacheOptions>(config.GetSection("Cache"));
services.Configure<RedisOptions>(config.GetSection("Cache:Providers:Redis"));
services.AddSingleton<MemoryCacheService>();
services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(...));
services.AddSingleton<RedisCacheService>();
services.AddSingleton<ICacheService, DefaultCacheService>();
services.AddSingleton<ICacheServiceFactory, CacheServiceFactory>();
```

نمونه استفاده
```
public class MySvc(ICacheService cache)
{
  public Task Save() => cache.SetAsync("key", new {x=1}, TimeSpan.FromMinutes(2));
}

public class MyOther(ICacheServiceFactory factory)
{
  public Task SaveToMemory()
    => factory.Get(CacheProviderKind.Memory).SetAsync("k", 1, TimeSpan.FromSeconds(10));
}
```

نکات
- کلیدها باید به‌صورت کامل از لایه مصرف‌کننده ساخته و پاس داده شوند.
- هیچ وابستگی بین پیاده‌سازی‌ها وجود ندارد؛ هر کدام فقط `ICacheService` را پیاده می‌کند.
