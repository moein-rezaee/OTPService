using FluentValidation;
using FluentValidation.AspNetCore;
using StackExchange.Redis;
using CacheExtension.Abstractions;
using CacheExtension.Core;
using CacheExtension.Memory;
using CacheExtension.Redis;
using SmsExtension.Abstractions;
using SmsExtension.Provider.Kavenegar;
using SmsExtension.Core;
using SmsExtension.Provider.Farapayamak;
using OTPService.Application.Features.OTP.Commands.SendCode;
using OTPService.Api.Swagger;
using DotNetEnv;
using OTPService.Domain.Interfaces;
using OTPService.Application.Features.OTP.Services;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => options.ConfigureSwagger());

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(SendCodeCommand).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<SendCodeCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddMemoryCache();

var cacheDefault = new DefaultCacheOptions();
builder.Configuration.GetSection("Cache").Bind(cacheDefault);
builder.Services.Configure<DefaultCacheOptions>(opt =>
{
    opt.DefaultProvider = builder.Configuration.GetValue<string>("Cache:DefaultProvider") ?? cacheDefault.DefaultProvider;
});

// Register Redis (if configured) and Memory, select default
var redisSection = builder.Configuration.GetSection("Cache:Providers:Redis");
var redisOptions = new RedisOptions();
redisSection.Bind(redisOptions);
var redisConfigured = redisSection.Exists();

if (redisConfigured)
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    {
        var cfg = new ConfigurationOptions();
        cfg.EndPoints.Add(redisOptions.Host, redisOptions.Port);
        cfg.User = redisOptions.Username;
        cfg.Password = redisOptions.Password;
        return ConnectionMultiplexer.Connect(cfg);
    });
    builder.Services.Configure<RedisOptions>(redisSection);
}

builder.Services.AddScoped<ICacheServiceFactory, CacheServiceFactory>();
builder.Services.AddScoped<MemoryCacheService>();
if (redisConfigured)
    builder.Services.AddScoped<RedisCacheService>();

var cacheDefaultProvider = (builder.Configuration.GetValue<string>("Cache:DefaultProvider") ?? cacheDefault.DefaultProvider).Trim();
if (string.Equals(cacheDefaultProvider, "Redis", StringComparison.OrdinalIgnoreCase) && redisConfigured)
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
else
    builder.Services.AddScoped<ICacheService, MemoryCacheService>();

// Configure SMS providers (register configured, pick default, expose factory)
var kavenegarSection = builder.Configuration.GetSection("Sms:Providers:Kavenegar");
var farapayamakSection = builder.Configuration.GetSection("Sms:Providers:Farapayamak");
var kavenegarConfigured = !string.IsNullOrWhiteSpace(kavenegarSection["ApiKey"]) && !string.IsNullOrWhiteSpace(kavenegarSection["Sender"]);
var farapayamakConfigured = !string.IsNullOrWhiteSpace(farapayamakSection["Username"]) && !string.IsNullOrWhiteSpace(farapayamakSection["Password"]) && (!string.IsNullOrWhiteSpace(farapayamakSection["From"]) || !string.IsNullOrWhiteSpace(farapayamakSection["Sender"]));

if (kavenegarConfigured)
    builder.Services.Configure<KavenegarOptions>(kavenegarSection);

if (farapayamakConfigured)
{
    builder.Services.Configure<FarapayamakOptions>(opt =>
    {
        opt.Username = farapayamakSection["Username"] ?? string.Empty;
        opt.Password = farapayamakSection["Password"] ?? string.Empty;
        opt.From = farapayamakSection["From"] ?? farapayamakSection["Sender"] ?? string.Empty;
    });
}

builder.Services.Configure<DefaultProviderOptions>(o =>
{
    o.DefaultProvider = builder.Configuration.GetValue<string>("Sms:DefaultProvider")
        ?? builder.Configuration.GetValue<string>("Sms:Provider")
        ?? "Farapayamak";
});

builder.Services.AddScoped<ISmsProviderFactory, SmsProviderFactory>();
builder.Services.AddScoped<ISmsProvider, DefaultSmsProvider>();
builder.Services.AddScoped<KavenegarSmsProvider>();
builder.Services.AddScoped<FarapayamakSmsProvider>();

builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "OTP Service API v1");
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
