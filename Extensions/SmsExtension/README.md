# SmsExtension (Wrapper ارسال پیامک)

هدف
- فراهم‌کردن یک API یکسان برای ارسال پیامک با پرووایدرهای مختلف (Kavenegar/Farapayamak).
- انتخاب پرووایدر پیش‌فرض از کانفیگ و امکان تغییر پرووایدر در زمان کدنویسی با Factory.

ساختار پوشه‌ها
- Abstractions
  - `ISmsProvider`: قرارداد ارسال پیام (`Task<SmsResult> SendAsync(SmsMessage, ct)`).
  - `ISmsProviderFactory`: گرفتن کلاینت پیش‌فرض/خاص؛ `GetDefault()`, `Get(SmsProviderKind)`, `GetAll()`.
- Core
  - `SmsMessage`, `SmsResult`: مدل‌های ساده پیام/نتیجه.
  - `DefaultProviderOptions`: تعیین `DefaultProvider` (Farapayamak/Kavenegar).
  - `DefaultSmsProvider`: Router که یک‌بار بر اساس کانفیگ انتخاب و سپس دلیگیت می‌کند.
  - `SmsProviderFactory`: ساخت/بازگردانی کلاینت‌ها بر اساس enum `SmsProviderKind`.
  - `SmsProviderKind`: `Kavenegar`, `Farapayamak`.
- Provider.Kavenegar
  - `KavenegarSmsProvider`, `KavenegarOptions { ApiKey, Sender }`.
- Provider.Farapayamak
  - `FarapayamakSmsProvider`, `FarapayamakOptions { Username, Password, From }`.

نصب پکیج‌های پرووایدرها (در csproj این لایبرری انجام شده)
- `KavenegarDotNetCore` (برای Kavenegar)
- `Farapayamak.NetCore` (برای Farapayamak)

کانفیگ (appsettings)
```
"Sms": {
  "DefaultProvider": "Farapayamak",
  "Providers": {
    "Kavenegar": { "ApiKey": "...", "Sender": "1000xxxx" },
    "Farapayamak": { "Username": "user", "Password": "pass", "From": "5000xxxx" }
  }
}
```

ثبت در DI (نمونه)
```
// options
services.Configure<DefaultProviderOptions>(config.GetSection("Sms"));
services.Configure<KavenegarOptions>(config.GetSection("Sms:Providers:Kavenegar"));
services.Configure<FarapayamakOptions>(config.GetSection("Sms:Providers:Farapayamak"));

// concrete providers
services.AddScoped<KavenegarSmsProvider>();
services.AddScoped<FarapayamakSmsProvider>();

// default provider + factory
services.AddScoped<ISmsProvider, DefaultSmsProvider>();
services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
```

نمونه استفاده
- استفاده از پیش‌فرض:
```
public class MySvc(ISmsProvider sms)
{
  public Task Send() => sms.SendAsync(new SmsMessage{ Mobile = "09...", Text = "..." });
}
```
- تغییر پرووایدر در زمان اجرا:
```
public class MySvc(ISmsProviderFactory factory)
{
  public Task SendKavenegar()
    => factory.Get(SmsProviderKind.Kavenegar)
              .SendAsync(new SmsMessage{ Mobile = "09...", Text = "..." });
}
```

نکات
- هیچ وابستگی بین پرووایدرها وجود ندارد؛ هرکدام فقط `ISmsProvider` را پیاده می‌کنند.
- خروجی همه پرووایدرها به `SmsResult` نگاشت می‌شود (Success/ErrorMessage/MessageId).
