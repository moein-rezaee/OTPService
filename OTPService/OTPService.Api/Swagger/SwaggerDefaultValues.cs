using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OTPService.Api.Swagger;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        if (operation.Responses.Count == 0)
        {
            operation.Responses.Add("200", new OpenApiResponse { Description = "عملیات با موفقیت انجام شد" });
            operation.Responses.Add("400", new OpenApiResponse { Description = "درخواست نامعتبر است" });
            operation.Responses.Add("401", new OpenApiResponse { Description = "عدم احراز هویت" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "دسترسی غیر مجاز" });
            operation.Responses.Add("500", new OpenApiResponse { Description = "خطای داخلی سرور" });
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null && description.DefaultValue != null && description.ModelMetadata?.ModelType != null)
            {
                var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}
