using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NotificationService.Api.Swagger;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Notification Service API",
            Version = "v1",
            Description = "Notification service endpoints"
        });

        options.OperationFilter<SwaggerDefaultValues>();
    }
}
