using System.Reflection;
using BuildingBlocks.Swagger.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace BuildingBlocks.Swagger;

public static class Extensions
{
    private const string SwaggerJsonPath = "/swagger/{documentName}/swagger.json";


    /// <summary>
    ///     Adds and configures Swagger services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="openApiInfoModel">The OpenApi information model containing the metadata for Swagger documentation.</param>
    /// <param name="assemblies">The list of assemblies to scan for XML comments.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddCustomSwagger(
        this IServiceCollection services,
        OpenApiInfo openApiInfoModel,
        params Assembly[] assemblies
    )
    {
        var scanAssemblies = assemblies.Any() ?
                                 assemblies :
                                 new[]
                                 {
                                     Assembly.GetExecutingAssembly(),
                                 };

        services.AddEndpointsApiExplorer();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(
            options =>
            {
                foreach (var openApiInfo in options.SwaggerGeneratorOptions.SwaggerDocs.Values)
                {
                    openApiInfo.Title = openApiInfoModel.Title;
                    openApiInfo.Description = openApiInfoModel.Description;
                    openApiInfo.Version = openApiInfoModel.Version;
                    openApiInfo.Contact = openApiInfoModel.Contact;
                    openApiInfo.License = openApiInfoModel.License;
                    openApiInfo.TermsOfService = openApiInfoModel.TermsOfService;
                    openApiInfo.Extensions = openApiInfoModel.Extensions;
                }

                options.DescribeAllParametersInCamelCase();
                options.AddEnumsWithValuesFixFilters();
                options.OperationFilter<SwaggerDefaultValues>();

                foreach (var assembly in scanAssemblies)
                {
                    var xmlFile = XmlCommentsFilePath(assembly);
                    if (File.Exists(xmlFile)) options.IncludeXmlComments(xmlFile);
                }

                options.SchemaFilter<IgnoreReadOnlySchemaFilter>();
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.EnableAnnotations();
            });

        services.Configure<SwaggerGeneratorOptions>(o => o.InferSecuritySchemes = true);

        return services;
    }

    private static string XmlCommentsFilePath(Assembly assembly)
    {
        var basePath = Path.GetDirectoryName(assembly.Location);
        var fileName = assembly.GetName().Name + ".xml";
        return Path.Combine(basePath!, fileName);
    }

    /// <summary>
    ///     Configures Swagger in an ASP.NET Core application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The configured web application.</returns>
    public static IApplicationBuilder UseCustomSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(
            options =>
            {
                var descriptions = app.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    options.DefaultModelsExpandDepth(-1);
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });

        return app;
    }
}
