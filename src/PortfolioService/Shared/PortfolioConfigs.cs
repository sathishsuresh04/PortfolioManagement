using BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PortfolioService.Portfolios.Features.DeletePortfolioById;
using PortfolioService.Portfolios.Features.GetPortfolioById;
using PortfolioService.Portfolios.Features.GetTotalPortfolioValue;
using PortfolioService.Shared.Extensions;

namespace PortfolioService.Shared;

public static class PortfolioConfigs
{
    private const string PortfoliosTag = "Portfolios";
    private const string Portfolios = "portfolios";
    private static string PortfolioPrefixUri => $"{EndpointConfig.BaseApiPath}/{Portfolios}";

    public static WebApplicationBuilder AddPortfolioServices(this WebApplicationBuilder builder)
    {
        builder.AddInfrastructure();
        return builder;
    }

    public static async Task<WebApplication> UsePortfolioServices(this WebApplication app)
    {
        await app.UseInfrastructure();
        return app;
    }

    public static IEndpointRouteBuilder MapPortfolioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/", () => "Portfolio Service").ExcludeFromDescription();
        endpoints.MapPortfolioModuleEndpoints();
        return endpoints;
    }

    private static IEndpointRouteBuilder MapPortfolioModuleEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var portfoliosV1 = endpoints.NewVersionedApi(PortfoliosTag)
            .MapGroup(PortfolioPrefixUri)
            .HasDeprecatedApiVersion(0.9)
            .HasApiVersion(1.0);

        portfoliosV1.MapGetPortfolioByIdEndpoint();
        portfoliosV1.MapGetTotalPortfolioValueEndpoint();
        portfoliosV1.MapDeletePortfolioByIdEndpoint();

        return endpoints;
    }
}
