using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OTPService.Api.Swagger;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this SwaggerGenOptions options)
    {
        // Configure Swagger document
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "OTP Service API",
            Version = "1.0",
            Description = "سرویس مدیریت رمزهای یکبار مصرف",
            Contact = new OpenApiContact
            {
                Name = "تیم توسعه",
                Email = "dev@example.com"
            },
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("https://opensource.org/licenses/MIT")
            }
        });
        
        // Add security definitions and requirements if needed
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        // Enable annotations for all controllers
        options.EnableAnnotations();

        // Add XML comments
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
        
        // Add operation filters for consistent documentation
        options.OperationFilter<SwaggerDefaultValues>();
    }
}