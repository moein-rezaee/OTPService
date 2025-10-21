# OTPService (DotNet 8.0 | WebApi)

سرویس OTP برای ارسال و تایید کد یکبارمصرف با تکیه بر دو اکستنشن ماژولار:
- SmsExtension: رپر ارسال پیامک (Kavenegar/Farapayamak)
- CacheExtension: رپر کش (Redis/Memory)

ویژگی‌ها
- SOLID و Clean Code، لایه‌بندی Abstractions/Core/Providers
- انتخاب پرووایدر پیش‌فرض از کانفیگ، و امکان تغییر در زمان کدنویسی با Factory
- میدل‌ویر سراسری مدیریت استثناها (Validation/Server)

ساختار مهم
- Services/OtpManager.cs: منطق ارسال/اعتبارسنجی OTP (با `ISmsProvider` و `ICacheService`)
- Controllers/SmsController.cs: اکشن‌ها
  - GET `/SendCode/{mobile}` → ارسال OTP
  - GET `/VerifyCode/{mobile}/{code}` → تایید OTP
- Extensions/SmsExtension: اکستنشن ارسال SMS
- Extensions/CacheExtension: اکستنشن کش

پیش‌نیازها
- .NET SDK 8
- برای Redis: یک سرور Redis (لوکال یا ریموت)

کانفیگ
- appsettings.json
  - Sms
    - DefaultProvider: `Farapayamak` یا `Kavenegar`
    - Providers
      - Kavenegar: `ApiKey`, `Sender`
      - Farapayamak: `Username`, `Password`, `From`
  - Cache
    - DefaultProvider: `Redis` یا `Memory`
    - Providers.Redis: `Host`, `Port`, `Username`, `Password`, `DefaultDatabase`

DI (ثبت سرویس‌ها) – نمونه Program.cs
- Sms
  - `ISmsProvider` → DefaultSmsProvider (پیش‌فرض از کانفیگ)
  - `ISmsProviderFactory` → SmsProviderFactory (برای تغییر در زمان اجرا)
- Cache
  - `ICacheService` → DefaultCacheService (پیش‌فرض از کانفیگ)
  - `ICacheServiceFactory` → CacheServiceFactory

نمونه استفاده در سرویس
- ارسال کد: `OtpManager.SendCode(dto)` → کد را ارسال و به‌مدت 2 دقیقه در کش ذخیره می‌کند.
- تایید کد: `OtpManager.VerifyCode(mobile, code)` → از کش خوانده و مقایسه می‌کند.

تغییر پرووایدر در زمان اجرا (اختیاری)
- با تزریق Factory‌ها می‌توانید پرووایدر خاص را بگیرید و استفاده کنید.
  - SMS: `ISmsProviderFactory.Get(SmsProviderKind.Kavenegar|Farapayamak)`
  - Cache: `ICacheServiceFactory.Get(CacheProviderKind.Redis|Memory)`

اجرای سرویس
- Development: `dotnet run`
- Docker Compose (در صورت نیاز): فایل‌های docker-compose موجود را اصلاح و اجرا کنید.

مستندات اکستنشن‌ها
- توضیحات کامل و مثال‌های کد در:
  - `Extensions/SmsExtension/README.md`
  - `Extensions/CacheExtension/README.md`
