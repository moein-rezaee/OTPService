using FluentValidation;
using OTPService.DTOs;
using OTPService.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IValidator<SendCodeDto>, SendCodeValidator>();
builder.Services.AddScoped<IValidator<VerifyCodeDto>, VerifyCodeValidator>();
// Session
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
    options.InstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value;
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
