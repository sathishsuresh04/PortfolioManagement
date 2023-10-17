using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Swagger;
using Microsoft.AspNetCore.Builder;

namespace PortfolioService.Shared.Extensions;

public static class UseInfrastructureExtensions
{
    public static async Task<WebApplication> UseInfrastructure(this WebApplication app)
    {
        app.UseCustomProblemDetails();
        app.UseRateLimiter();
        app.UseCustomSwagger();
        await SeedDataAsync(app.Services);
        return app;
    }

    private static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();
        foreach (var seeder in seeders) await seeder.SeedAllAsync();
    }
}
