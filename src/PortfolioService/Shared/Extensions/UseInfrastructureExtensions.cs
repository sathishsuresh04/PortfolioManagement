using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;

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
