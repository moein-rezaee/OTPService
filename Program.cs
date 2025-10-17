using FluentValidation;
using OTPService.DTOs;
using OTPService.Validations;
using SmsExtension.Core;
using SmsExtension.Provider.Kavenegar;
using SmsExtension.Provider.Farapayamak;
using SmsExtension.Abstractions;
using CacheExtension.Abstractions;
using CacheExtension.Memory;
using CacheExtension.Redis;
using CacheExtension.Core;
using OTPService.Middleware;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<SendCodeDto>, SendCodeValidator>();
builder.Services.AddScoped<IValidator<VerifyCodeRequestDto>, VerifyCodeRequestValidator>();
// Sms providers and options
builder.Services.Configure<DefaultProviderOptions>(builder.Configuration.GetSection("Sms"));
builder.Services.Configure<KavenegarOptions>(builder.Configuration.GetSection("Sms:Providers:Kavenegar"));
builder.Services.Configure<FarapayamakOptions>(builder.Configuration.GetSection("Sms:Providers:Farapayamak"));
builder.Services.AddScoped<KavenegarSmsProvider>();
builder.Services.AddScoped<FarapayamakSmsProvider>();
// Router and factory for SMS providers
builder.Services.AddScoped<ISmsProvider, DefaultSmsProvider>();
builder.Services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();

// Application services
builder.Services.AddScoped<OTPService.Services.OtpManager>();
// Caching
builder.Services.AddMemoryCache();
// Cache provider registrations
builder.Services.Configure<DefaultCacheOptions>(builder.Configuration.GetSection("Cache"));
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Cache:Providers:Redis"));
builder.Services.AddSingleton<MemoryCacheService>();
// Redis connection multiplexer via discrete options
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var opts = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
    var cfg = new ConfigurationOptions
    {
        AbortOnConnectFail = false
    };
    cfg.EndPoints.Add(opts.Host ?? "localhost", opts.Port);
    if (!string.IsNullOrWhiteSpace(opts.Username)) cfg.User = opts.Username;
    if (!string.IsNullOrWhiteSpace(opts.Password)) cfg.Password = opts.Password;
    if (opts.DefaultDatabase.HasValue) cfg.DefaultDatabase = opts.DefaultDatabase.Value;
    return ConnectionMultiplexer.Connect(cfg);
});
builder.Services.AddSingleton<RedisCacheService>();
// Router and factory for Cache services
builder.Services.AddSingleton<ICacheService, DefaultCacheService>();
builder.Services.AddSingleton<ICacheServiceFactory, CacheServiceFactory>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
