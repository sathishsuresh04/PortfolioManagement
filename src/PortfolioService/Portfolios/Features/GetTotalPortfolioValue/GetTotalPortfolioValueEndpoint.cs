using System.ComponentModel;
using BuildingBlocks.Web.Minimal;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Serilog.Context;

namespace PortfolioService.Portfolios.Features.GetTotalPortfolioValue;


    internal static class GetTotalPortfolioValueEndpoint
    {
        internal static RouteHandlerBuilder MapGetTotalPortfolioValueEndpoint(this IEndpointRouteBuilder builder)
        {
            return builder.MapGet(
                    "/value/{id}/{currency}", // Adding a default value for currency
                    async Task<Results<Ok<GetTotalPortfolioValueResponse>, ValidationProblem, NotFound>> (
                        [FromRoute] string id,
                        [FromRoute(Name = "currency")][DefaultValue("USD")] string currency,
                        IMediator mediator,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        var getTotalPortfolioValue = new GetTotalPortfolioValue(id, currency);
                        using (LogContext.PushProperty("Endpoint", nameof(GetTotalPortfolioValueEndpoint)))
                        {
                            using (LogContext.PushProperty("PortfolioId", getTotalPortfolioValue.Id))
                            {
                                var data = await mediator.Send(getTotalPortfolioValue, cancellationToken);

                                return TypedResults.Ok(data);
                            }
                        }
                    })
                .WithName(nameof(GetTotalPortfolioValueEndpoint))
                .WithDisplayName(nameof(GetTotalPortfolioValueEndpoint).Humanize())
                .WithSummaryAndDescription(
                    "Retrieve total value of a portfolio",
                    "Gets the total value of the portfolio identified by the specified ID, in the specified currency")
                .Produces<OkResult>("Total portfolio value retrieved successfully.", StatusCodes.Status200OK)
                .ProducesValidationProblem("Invalid input for total portfolio value retrieval.")
                .ProducesProblem(
                    "An error occurred while retrieving the total portfolio value.",
                    StatusCodes.Status500InternalServerError)
                .MapToApiVersion(1.0);
        }
    }

