using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using Shared.CacheService.Abstractions;
using Shared.CacheService.Core;
using Shared.CacheService.Memory;
using Shared.CacheService.Redis;
using Shared.SmsProvider.Abstractions;
using Shared.SmsProvider.Kavenegar;
using Shared.SmsProvider.Core;
using Shared.SmsProvider.Farapayamak;
using OTPService.Application.Features.OTP.Commands.SendCode;
using DomainService = OTPService.Domain.Interfaces;
using ApplicationService = OTPService.Application.Features.OTP.Services;
using OTPService.Domain.Interfaces;
using OTPService.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using StackExchange.Redis;
using Serilog;

// Load Environment Variables
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure environment-specific settings
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});
var logger = loggerFactory.CreateLogger<Program>();

// Add API configuration
builder.Services.AddControllers();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with centralized documentation
builder.Services.AddSwaggerGen(options => options.ConfigureSwagger());

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(SendCodeCommand).Assembly);
});

// Add Validators
builder.Services.AddValidatorsFromAssemblyContaining<SendCodeCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Add Memory Cache
builder.Services.AddMemoryCache();

// Configure Cache Provider
var cacheOptions = new DefaultCacheOptions();
builder.Configuration.GetSection("Cache").Bind(cacheOptions);

builder.Services.Configure<DefaultCacheOptions>(options => 
{
    options.DefaultProvider = cacheOptions.DefaultProvider;
    options.DefaultExpirationMinutes = cacheOptions.DefaultExpirationMinutes;
});

if (cacheOptions.DefaultProvider == CacheProviderKind.Redis)
{
    var redisSection = builder.Configuration.GetSection("Cache:Providers:Redis");
    var redisOptions = new RedisOptions();
    redisSection.Bind(redisOptions);

    builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
    {
        try
        {
            var mux = ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            logger.LogInformation("Successfully connected to Redis at {Host}:{Port}", redisOptions.Host, redisOptions.Port);
            return mux;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to connect to Redis at {Host}:{Port}. Falling back to Memory Cache", redisOptions.Host, redisOptions.Port);
            throw;
        }
    });

    builder.Services.Configure<RedisOptions>(redisSection);
    builder.Services.AddScoped<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddScoped<ICacheService, MemoryCacheService>();
}

// Configure SMS Provider
var smsProvider = builder.Configuration.GetValue<string>("Sms:DefaultProvider") ?? "Kavenegar";
var smsProviderKind = Enum.Parse<SmsProviderKind>(smsProvider, true);

if (smsProviderKind == SmsProviderKind.Kavenegar)
{
    var kavenegarSection = builder.Configuration.GetSection("Sms:Providers:Kavenegar");
    builder.Services.Configure<KavenegarOptions>(kavenegarSection);
    builder.Services.AddScoped<ISmsProvider, KavenegarSmsProvider>();
}
else if (smsProviderKind == SmsProviderKind.Farapayamak)
{
    var farapayamakSection = builder.Configuration.GetSection("Sms:Providers:Farapayamak");
    builder.Services.Configure<FarapayamakOptions>(farapayamakSection);
    builder.Services.AddScoped<ISmsProvider, FarapayamakSmsProvider>();
}
else
{
    throw new InvalidOperationException($"Unsupported SMS provider: {smsProvider}");
}

// Register Services
builder.Services.AddScoped<DomainService.IOtpService, ApplicationService.OtpService>();

// Add CORS
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

// Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OTP Service API");
        options.RoutePrefix = string.Empty; // To serve the Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers()
   .RequireAuthorization();

app.Run();
