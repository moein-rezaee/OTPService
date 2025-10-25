using AuthenticationService.Application.Features.Auth.Commands.Refresh;
using AuthenticationService.Application.Features.Auth.Commands.Send;
using AuthenticationService.Application.Features.Auth.Commands.Verify;
using AuthenticationService.Application.Features.Auth.Services;
using AuthenticationService.Infrastructure.Data;
using AuthenticationService.Infrastructure.Otp;
using CacheExtension;
using DotNetEnv;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SendCommand).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<SendCommand>();
builder.Services.AddFluentValidationAutoValidation();

// Options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Cache (for blacklist/ratelimit if needed)
builder.Services.AddCacheExtension(builder.Configuration);

// EF Core Postgres
var pgHost = builder.Configuration["Database:Postgres:Host"] ?? "localhost";
var pgPort = int.TryParse(builder.Configuration["Database:Postgres:Port"], out var p) ? p : 5432;
var pgDb = builder.Configuration["Database:Postgres:Database"] ?? "authdb";
var pgUser = builder.Configuration["Database:Postgres:Username"] ?? "postgres";
var pgPass = builder.Configuration["Database:Postgres:Password"] ?? "postgres";
var connStr = $"Host={pgHost};Port={pgPort};Database={pgDb};Username={pgUser};Password={pgPass};";
builder.Services.AddDbContext<AuthDbContext>(o => o.UseNpgsql(connStr));
builder.Services.AddScoped<Microsoft.EntityFrameworkCore.DbContext, AuthDbContext>();
builder.Services.AddScoped<AuthenticationService.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

// OTP HTTP client
builder.Services.AddHttpClient<AuthenticationService.Domain.Interfaces.IOtpClient, OtpHttpClient>();

// JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Ensure DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();

