using Asp.Versioning;

namespace BuildingBlocks.Web;

/// <summary>
///     Provides extension methods for adding custom versioning to a IServiceCollection.
/// </summary>
public static class VersioningExtensions
{
    /// <summary>
    ///     Adds custom versioning configuration to the IServiceCollection.
    /// </summary>
    /// <param name="services">The IServiceCollection.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddCustomVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(
                options =>
                {
                    // Reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions".
                    options.ReportApiVersions = true;

                    // Defines how an API version is read from the current HTTP request.
                    // The default `ApiVersionReader` combines the `HeaderApiVersionReader`, `QueryStringApiVersionReader`, and `UrlSegmentApiVersionReader`.
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader("api-version"),
                        new QueryStringApiVersionReader(),
                        new UrlSegmentApiVersionReader());

                    // AssumeDefaultVersionWhenUnspecified should only be enabled when supporting legacy services that did not previously support API versioning.
                    // Forcing existing clients to specify an explicit API version for an existing service introduces a breaking change.
                    // This option assumes a default version when no version is specified in the request.
                    options.AssumeDefaultVersionWhenUnspecified = true;

                    // The default API version to use when none is specified in the request.
                    options.DefaultApiVersion = new ApiVersion(1, 0);

                    // Configures versioning policies.
                    options.Policies
                        .Sunset(0.9) // Sets the sunset version for the API.
                        .Effective(DateTimeOffset.Now.AddDays(60)) // Sets the effective date for the API.
                        .Link("policy.html") // Sets the link to the versioning policy documentation.
                        .Title("Versioning Policy") // Sets the title of the versioning policy.
                        .Type("text/html"); // Sets the media type of the versioning policy documentation.
                })
            .AddApiExplorer(
                options =>
                {
                    // Adds the versioned API explorer, which also adds the IApiVersionDescriptionProvider service.
                    // The specified format code will format the version as "'v'major[.minor][-status]".
                    options.GroupNameFormat = "'v'VVV";

                    // This option is only necessary when versioning by URL segment.
                    // It substitutes the API version in the URL to control the format of the API version in route templates.
                    options.SubstituteApiVersionInUrl = true;
                })
            .AddMvc()
            .EnableApiVersionBinding(); // Enables binding ApiVersion as an endpoint callback parameter.

        return services;
    }
}
