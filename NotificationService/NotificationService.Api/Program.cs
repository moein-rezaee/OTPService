using FluentValidation;
using FluentValidation.AspNetCore;
using SmsExtension;
using NotificationService.Application.Features.Notifications.Commands.Send;
using NotificationService.Api.Swagger;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => options.ConfigureSwagger());

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(SendSmsCommand).Assembly);
});

builder.Services.AddValidatorsFromAssemblyContaining<SendSmsCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddSmsExtension(builder.Configuration);

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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification Service API v1");
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
