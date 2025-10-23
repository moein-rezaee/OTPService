using FluentValidation;
using FluentValidation.AspNetCore;
using CacheExtension;
using SmsExtension;
using SmsExtension.Abstractions;
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

builder.Services.AddCacheExtension(builder.Configuration);
builder.Services.AddSmsExtension(builder.Configuration);

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
