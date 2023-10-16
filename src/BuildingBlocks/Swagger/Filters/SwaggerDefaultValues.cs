using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger.Filters;

/// <summary>
///     An implementation of <see cref="IOperationFilter" /> that applies default values and descriptions to Swagger
///     operations.
/// </summary>
internal sealed class SwaggerDefaultValues : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // Check if the API endpoint is marked as deprecated
        operation.Deprecated |= apiDescription.IsDeprecated();

        // Remove unsupported response content types from the operation
        foreach (var responseType in context.ApiDescription.SupportedResponseTypes)
        {
            var responseKey = responseType.IsDefaultResponse ?
                                  "default" :
                                  responseType.StatusCode.ToString(CultureInfo.InvariantCulture);
            var response = operation.Responses[responseKey];

            foreach (var contentType in response.Content.Keys)
            {
                if (responseType.ApiResponseFormats.All(
                        x =>
                            !string.Equals(x.MediaType, contentType, StringComparison.OrdinalIgnoreCase)))
                    response.Content.Remove(contentType);
            }
        }

        // Update parameter descriptions and defaults
        if (operation.Parameters == null) return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions.FirstOrDefault(
                p =>
                    string.Equals(p.Name, parameter.Name, StringComparison.OrdinalIgnoreCase));

            // Set the parameter description if available
            parameter.Description ??= description?.ModelMetadata?.Description;

            // Set the parameter default value if available
            if (parameter.Schema.Default == null &&
                description?.DefaultValue != null &&
                description.DefaultValue is not DBNull &&
                description.ModelMetadata is
                {
                } modelMetadata)
            {
                var json = JsonSerializer.Serialize(description.DefaultValue, modelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            // Set the parameter required flag based on the description
            if (description != null) parameter.Required |= description.IsRequired;
        }
    }
}
