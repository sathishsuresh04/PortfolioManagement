using BuildingBlocks.Web.Minimal;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PortfolioService.Portfolios.Dtos;
using Serilog.Context;

namespace PortfolioService.Portfolios.Features.GetPortfolioById;

internal static class GetPortfolioByIdEndpoint
{
    internal static RouteHandlerBuilder MapGetPortfolioByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapGet(
                "/{id}",
                async Task<Results<Ok<PortfolioDto>, ValidationProblem, NotFound>> (
                    [FromRoute] string id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var query = new GetPortfolioById(id);
                    using (LogContext.PushProperty("Endpoint", nameof(GetPortfolioByIdEndpoint)))
                    {
                        using (LogContext.PushProperty("PortfolioId", query.Id))
                        {
                            var data = await mediator.Send(query, cancellationToken);

                            return TypedResults.Ok(data);
                        }
                    }
                })
            .WithName(nameof(GetPortfolioByIdEndpoint))
            .WithDisplayName(nameof(GetPortfolioByIdEndpoint).Humanize())
            .WithSummaryAndDescription(
                "Retrieves a specific portfolio",
                "Retrieves the portfolio identified by the specified ID")
            .Produces<OkResult>("Portfolio retrieved successfully.", StatusCodes.Status200OK)
            .ProducesValidationProblem("Invalid input for portfolio retrieval.")
            .ProducesProblem(
                "An error occurred while retrieving the portfolio.",
                StatusCodes.Status500InternalServerError)
            .MapToApiVersion(1.0);
    }
}
