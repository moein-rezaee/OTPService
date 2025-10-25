using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AuthenticationService.Api.Swagger;

public static class SwaggerConfiguration
{
    public static void ConfigureSwagger(this SwaggerGenOptions options)
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Authentication Service API",
            Version = "v1",
            Description = "Auth endpoints"
        });
        options.OperationFilter<SwaggerDefaultValues>();
    }
}

