using FluentValidation;
using OTPService.DTOs;
using OTPService.Validations;
using SmsExtension.Core;
using SmsExtension.Provider.Kavenegar;
using SmsExtension.Provider.Farapayamak;
using SmsExtension;
using OTPService.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<SendCodeDto>, SendCodeValidator>();
builder.Services.AddScoped<IValidator<VerifyCodeDto>, VerifyCodeValidator>();
// Sms providers and options
builder.Services.Configure<DefaultProviderOptions>(builder.Configuration.GetSection("Sms"));
builder.Services.Configure<KavenegarOptions>(builder.Configuration.GetSection("Sms:Providers:Kavenegar"));
builder.Services.Configure<FarapayamakOptions>(builder.Configuration.GetSection("Sms:Providers:Farapayamak"));
builder.Services.AddScoped<KavenegarSmsProvider>();
builder.Services.AddScoped<FarapayamakSmsProvider>();
builder.Services.AddScoped<ISmsProvider, DefaultSmsProvider>();

// Application services
builder.Services.AddScoped<OTPService.Services.SmsService>();
// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".OTPServcie.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(2);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseGlobalExceptionHandling();
app.UseSession();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
