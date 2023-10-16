using BuildingBlocks.Web.Minimal;
using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Serilog.Context;

namespace PortfolioService.Portfolios.Features.DeletePortfolioById;

internal static class DeletePortfolioByIdEndpoint
{
    internal static RouteHandlerBuilder MapDeletePortfolioByIdEndpoint(this IEndpointRouteBuilder builder)
    {
        return builder.MapDelete(
                "/{id}",
                async Task<Results<NoContent, ValidationProblem, NotFound>> (
                    [FromRoute] string id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var command = new DeletePortfolioById(id);

                    using (LogContext.PushProperty("Endpoint", nameof(DeletePortfolioByIdEndpoint)))
                    {
                        using (LogContext.PushProperty("PortfolioId", command.Id))
                        {
                            await mediator.Send(command, cancellationToken);

                            return TypedResults.NoContent();
                        }
                    }
                })
            .WithName(nameof(DeletePortfolioByIdEndpoint))
            .WithDisplayName(nameof(DeletePortfolioByIdEndpoint).Humanize())
            .WithSummaryAndDescription(
                "Deletes a specific portfolio",
                "Deletes the portfolio identified by the specified ID")
            .Produces<NoContentResult>("Portfolio deleted successfully.", StatusCodes.Status204NoContent)
            .ProducesValidationProblem("Invalid input for portfolio deletion.")
            .ProducesProblem(
                "An error occurred while deleting the portfolio.",
                StatusCodes.Status500InternalServerError)
            .MapToApiVersion(1.0);
    }
}
