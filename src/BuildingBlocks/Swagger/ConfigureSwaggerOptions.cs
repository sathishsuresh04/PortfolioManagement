using System.Text;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BuildingBlocks.Swagger;

/// <summary>
///     Configuration options for SwaggerGenOptions to generate Swagger documents with API versioning.
/// </summary>
internal sealed class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private const string DeprecatedText = " This API version has been deprecated.";

    private readonly IApiVersionDescriptionProvider _provider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions" /> class.
    /// </summary>
    /// <param name="provider">The provider used to generate Swagger documents.</param>
    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    ///     Configures SwaggerGenOptions for API versioning.
    /// </summary>
    /// <param name="options">The SwaggerGenOptions to configure.</param>
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var versionDescription in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(versionDescription.GroupName, CreateVersionInfo(versionDescription));
    }

    /// <summary>
    ///     Configures SwaggerGenOptions for a specific named instance.
    /// </summary>
    /// <param name="name">The name of the instance.</param>
    /// <param name="options">The SwaggerGenOptions to configure.</param>
    public void Configure(string? name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var text = new StringBuilder("This application with OpenAPI, Swashbuckle, and API versioning.");

        var versionInfo = new OpenApiInfo {Version = description.ApiVersion.ToString(),};

        if (description.IsDeprecated) text.Append(DeprecatedText);

        if (description.SunsetPolicy is
            {
            } policy)
        {
            if (policy.Date is
                {
                } when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                foreach (var link in policy.Links)
                {
                    if (link.Type != "text/html") continue;

                    text.AppendLine();

                    if (link.Title.HasValue) text.Append(link.Title.Value).Append(": ");

                    text.Append(link.LinkTarget.OriginalString);
                }
            }
        }

        versionInfo.Description = text.ToString();
        return versionInfo;
    }
}
