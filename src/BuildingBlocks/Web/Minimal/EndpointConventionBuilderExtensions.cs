using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Web.Minimal;

public static class EndpointConventionBuilderExtensions
{
    /// <summary>
    ///     Specifies the response type and content type for the specified status code and adds an OpenAPI description for the
    ///     operation.
    /// </summary>
    /// <param name="builder">The RouteHandlerBuilder instance.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="responseType">The type of the response.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="additionalContentTypes">Additional content types supported by the response.</param>
    /// <returns>The updated RouteHandlerBuilder instance.</returns>
    public static RouteHandlerBuilder Produces(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        Type? responseType = null,
        string? contentType = null,
        params string[] additionalContentTypes
    )
    {
        builder.WithOpenApi(
            operation =>
            {
                operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
                return operation;
            });

        builder.Produces(statusCode, responseType, contentType, additionalContentTypes);

        return builder;
    }

    /// <summary>
    ///     Specifies the response type and content type for the specified status code and adds an OpenAPI description for the
    ///     operation.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="builder">The RouteHandlerBuilder instance.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <param name="additionalContentTypes">Additional content types supported by the response.</param>
    /// <returns>The updated RouteHandlerBuilder instance.</returns>
    public static RouteHandlerBuilder Produces<TResponse>(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        string? contentType = null,
        params string[] additionalContentTypes
    )
    {
        builder.WithOpenApi(
            operation =>
            {
                operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
                return operation;
            });

        builder.Produces<TResponse>(statusCode, contentType, additionalContentTypes);

        return builder;
    }

    /// <summary>
    ///     Specifies that the endpoint produces a problem details response for the specified status code and adds an OpenAPI
    ///     description for the operation.
    /// </summary>
    /// <param name="builder">The RouteHandlerBuilder instance.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <returns>The updated RouteHandlerBuilder instance.</returns>
    public static RouteHandlerBuilder ProducesProblem(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode,
        string? contentType = null
    )
    {
        builder.WithOpenApi(
            operation =>
            {
                operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
                return operation;
            });

        builder.ProducesProblem(statusCode, contentType);

        return builder;
    }

    /// <summary>
    ///     Specifies that the endpoint produces a validation problem response for the specified status code and adds an
    ///     OpenAPI description for the operation.
    /// </summary>
    /// <param name="builder">The RouteHandlerBuilder instance.</param>
    /// <param name="description">The description of the response.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="contentType">The content type of the response.</param>
    /// <returns>The updated RouteHandlerBuilder instance.</returns>
    public static RouteHandlerBuilder ProducesValidationProblem(
        this RouteHandlerBuilder builder,
        string description,
        int statusCode = StatusCodes.Status400BadRequest,
        string? contentType = null
    )
    {
        builder.WithOpenApi(
            operation =>
            {
                operation.Responses[statusCode.ToString(CultureInfo.InvariantCulture)].Description = description;
                return operation;
            });

        builder.ProducesValidationProblem(statusCode, contentType);

        return builder;
    }

    /// <summary>
    ///     Specifies the summary and description for the endpoint operation.
    /// </summary>
    /// <param name="builder">The RouteHandlerBuilder instance.</param>
    /// <param name="summary">The summary of the endpoint operation.</param>
    /// <param name="description">The description of the endpoint operation.</param>
    /// <returns>The updated RouteHandlerBuilder instance.</returns>
    public static RouteHandlerBuilder WithSummaryAndDescription(
        this RouteHandlerBuilder builder,
        string summary,
        string description
    )
    {
        builder.WithOpenApi(
            operation =>
            {
                operation.Summary = summary;
                operation.Description = description;
                return operation;
            });

        return builder;
    }
}
