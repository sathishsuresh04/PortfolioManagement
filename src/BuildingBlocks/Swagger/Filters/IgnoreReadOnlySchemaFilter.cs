using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger.Filters;

/// <summary>
///     Schema filter that removes read-only properties from the Swagger schema.
/// </summary>
internal sealed class IgnoreReadOnlySchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // Set ReadOnly to false for the schema
        schema.ReadOnly = false;

        if (schema.Properties == null) return;


        // Set ReadOnly to false for each property in the schema
        foreach (var property in schema.Properties) property.Value.ReadOnly = false;
    }
}
