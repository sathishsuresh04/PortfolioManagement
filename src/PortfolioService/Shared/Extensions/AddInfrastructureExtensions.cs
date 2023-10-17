using System.Threading.RateLimiting;
using BuildingBlocks.Abstractions.Persistence;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Caching;
using BuildingBlocks.Configurations;
using BuildingBlocks.Core.Mapster;
using BuildingBlocks.Core.Serialization;
using BuildingBlocks.Logging;
using BuildingBlocks.Swagger;
using BuildingBlocks.Validator;
using BuildingBlocks.Web;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PortfolioService.Portfolios.Data;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Data.Seed;
using PortfolioService.Shared.Abstractions;
using PortfolioService.Shared.Data;
using PortfolioService.Shared.Data.Abstractions;
using PortfolioService.Shared.Data.EntityMappings;
using PortfolioService.Shared.Data.Repositories;
using PortfolioService.Shared.Infrastructure;
using PortfolioService.Shared.Options;
using Refit;

namespace PortfolioService.Shared.Extensions;

public static class AddInfrastructureExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddConfigurations(builder.Configuration);
        builder.Host.AddCustomSerilog(builder.Configuration);
        var openApiModel = builder.Configuration.GetOptions<OpenApiInfo>(nameof(OpenApiInfo));
        builder.Services
            .Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true)
            .AddTransient<ISerializer, DefaultSerializer>()
            .AddCustomMediatR()
            .AddProblemDetails()
            .AddRateLimiter(
                options =>
                {
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                        httpContext =>
                            RateLimitPartition.GetFixedWindowLimiter(
                                httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                                partition => new FixedWindowRateLimiterOptions
                                             {
                                                 AutoReplenishment = true,
                                                 PermitLimit = 10,
                                                 QueueLimit = 0,
                                                 Window = TimeSpan.FromMinutes(1),
                                             }));
                })
            .AddPersistence()
            .AddEndpointsApiExplorer()
            .AddCustomSwagger(openApiModel, typeof(IPortfolioRoot).Assembly)
            .AddCustomVersioning()
            .AddCustomValidators(typeof(IPortfolioRoot).Assembly)
            .AddCustomMapster(typeof(IPortfolioRoot).Assembly)
            .AddRefitClient<IExchangeRateApiClient>()
            .ConfigureHttpClient(
                (sp, client) =>
                {
                    var options = sp.GetRequiredService<IOptions<ExchangeRateApiOptions>>();
                    client.BaseAddress = new Uri(options.Value.BaseApiAddress);
                    client.DefaultRequestHeaders.Add("apikey", options.Value.Token);
                });

        //now it is singleton since not maintaining any states , but in future it can be Transient or scoped
        builder.Services
            .AddSingleton<IStockService, StockService>()
            .AddTransient<ExchangeRateService>()
            .AddCustomCaching(builder.Configuration);
        TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
        return builder;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services
            .AddScoped<IPortfolioContext, PortfolioContext>()
            .AddScoped(typeof(IRepository<>), typeof(BaseRepository<>))
            .AddScoped<IPortfolioRepository, PortfolioRepository>()
            .AddScoped<IDataSeeder, PortfolioDataSeeder>();
        Mapping.Configure();
        return services;
    }

    private static IServiceCollection AddCustomMediatR(this IServiceCollection services)
    {
        services.AddMediatR(
            cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining(typeof(IPortfolioRoot));
                cfg.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });
        return services;
    }

    private static IServiceCollection AddConfigurations(
        this IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        serviceCollection.AddValidateOptions<MongoDbOptions>(
            nameof(MongoDbOptions),
            configuration);
        serviceCollection.AddValidateOptions<ExchangeRateApiOptions>(
            nameof(ExchangeRateApiOptions),
            configuration);
        return serviceCollection;
    }
}
