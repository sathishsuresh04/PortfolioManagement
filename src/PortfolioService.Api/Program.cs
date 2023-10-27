using Bogus;
using PortfolioService.Shared;
using Serilog;
using Serilog.Events;
using Spectre.Console;

namespace PortfolioService.Api;

internal static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Write(
            new FigletText("Portfolio Service").Centered().Color(Color.FromInt32(new Faker().Random.Int(1, 255))));
        Log.Logger = new LoggerConfiguration().MinimumLevel
            .Override("Microsoft", LogEventLevel.Information)
            .WriteTo.Console()
            .CreateBootstrapLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseDefaultServiceProvider(
                (context, options) =>
                {
                    var isDevMode =
                        context.HostingEnvironment.IsDevelopment() ||
                        context.HostingEnvironment.IsEnvironment("test") ||
                        context.HostingEnvironment.IsStaging();

                    // Handling Captive Dependency Problem
                    options.ValidateScopes = isDevMode;
                    options.ValidateOnBuild = isDevMode;
                });
            builder.AddPortfolioServices();
            var app = builder.Build();
            await app.UsePortfolioServices();
            app.MapPortfolioEndpoints();
            await app.RunAsync();
        }
        catch (Exception e) when (e is not OperationCanceledException && e.GetType().Name != "HostAbortedException")
        {
            Log.Fatal(e, "Program terminated unexpectedly!");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
