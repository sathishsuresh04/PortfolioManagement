using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace PortfolioService.Shared.Extensions;

public static class UseInfrastructureExtensions
{
    public static WebApplication UseInfrastructure(this WebApplication app)
    {
        app.UseCustomProblemDetails();
        app.UseRateLimiter();
        app.UseCustomSwagger();
        return app;
    }
}
