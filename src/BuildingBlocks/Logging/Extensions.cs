using BuildingBlocks.Configurations;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.AzureAnalytics;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.Grafana.Loki;
using Serilog.Sinks.SpectreConsole;

namespace BuildingBlocks.Logging;

/// <summary>
///     Provides extension methods for configuring custom Serilog logging in a web application.
/// </summary>
public static class RegistrationExtensions
{
    private static readonly string[] PropertiesAsLabels =
    {
        "app",
    };

    /// <summary>
    ///     Adds custom Serilog logging configuration to the web application.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configuration"></param>
    /// <param name="sectionName">The name of the configuration section for Serilog options. Default is "Serilog".</param>
    /// <param name="extraConfigure">An action to provide additional configuration for Serilog.</param>
    /// <returns> The web application builder</returns>
    public static IHostBuilder AddCustomSerilog(
        this IHostBuilder builder,
        IConfiguration configuration,
        string sectionName = "Serilog",
        Action<LoggerConfiguration>? extraConfigure = null
    )
    {
        var serilogOptions = configuration.GetOptions<SerilogOptions>(sectionName);

        // Configure Serilog
        builder.UseSerilog(
            (context, serviceProvider, loggerConfiguration) =>
            {
                extraConfigure?.Invoke(loggerConfiguration);
                var environment = context.HostingEnvironment;

                // Parse the minimum log level from the configuration file
                var defaultLogLevel = Enum.TryParse<LogEventLevel>(serilogOptions.MinimumLogLevel, out var result);
                // Determine the log level based on the parsed minimum log level
                var logLevel = GetLogLevel(result, loggerConfiguration);

                // Enrich log events with various properties
                loggerConfiguration
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                    .Enrich.WithCorrelationIdHeader()
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithMachineName()
                    .Enrich.WithExceptionDetails(
                        new DestructuringOptionsBuilder().WithDefaultDestructurers()
                            .WithDestructurers(
                                new[]
                                {
                                    new DbUpdateExceptionDestructurer(),
                                }));

                // Write logs to the console if configured to do so
                WriteToConsole(loggerConfiguration, serilogOptions);

                // Write logs to a file if configured to do so
                WriteToFile(loggerConfiguration, serilogOptions);

                // Send logs to Application Insights if configured to do so
                SendToApplicationInsights(loggerConfiguration, logLevel, serilogOptions);

                // Send logs to Log Analytics if configured to do so
                SendToLogAnalytics(loggerConfiguration, logLevel, serilogOptions);

                // Write logs to Elasticsearch if configured to do so
                WriteToElasticSearch(environment, loggerConfiguration, serilogOptions);

                // Write logs to Grafana Loki if configured to do so
                WriteToGrafanaLoki(loggerConfiguration, serilogOptions);

                // Write logs to Seq if configured to do so
                WriteToSeq(loggerConfiguration, serilogOptions);

                // Write logs to OpenTelemetry if configured to do so
                WriteToOpenTelemetry(loggerConfiguration, serilogOptions);
            });

        return builder;
    }

    /// <summary>
    ///     Determines the log level to use based on the parsed minimum log level and configures the logger accordingly.
    /// </summary>
    /// <param name="minimumLogLevelEnum">The parsed minimum log level.</param>
    /// <param name="logger">The logger to configure.</param>
    /// <returns>The determined log level.</returns>
    private static LogEventLevel GetLogLevel(
        LogEventLevel minimumLogLevelEnum,
        LoggerConfiguration logger
    )
    {
        LogEventLevel logLevel;
        switch (minimumLogLevelEnum)
        {
            case LogEventLevel.Verbose:
            case LogEventLevel.Debug:
                logger.MinimumLevel.Debug();
                logLevel = LogEventLevel.Debug;
                break;
            case LogEventLevel.Information:
                logger.MinimumLevel.Information();
                logLevel = LogEventLevel.Information;
                break;
            case LogEventLevel.Warning:
                logger.MinimumLevel.Warning();
                logLevel = LogEventLevel.Warning;
                break;
            case LogEventLevel.Error:
            case LogEventLevel.Fatal:
                logger.MinimumLevel.Error();
                logLevel = LogEventLevel.Error;
                break;
            default:
                logger.MinimumLevel.Information();
                logLevel = LogEventLevel.Information;
                break;
        }

        return logLevel;
    }

    private static void WriteToConsole(LoggerConfiguration logger, SerilogOptions serilogOptions)
    {
        if (!serilogOptions.ConsoleLogOptions.WriteToConsole) return;

        if (serilogOptions.ConsoleLogOptions.UseElasticsearchJsonFormatter)
            logger.WriteTo.Async(writeTo => writeTo.Console(new ExceptionAsObjectJsonFormatter(renderMessage: true)));
        else
            logger.WriteTo.Async(writeTo => writeTo.SpectreConsole(serilogOptions.LogTemplate));
    }

    private static void WriteToFile(LoggerConfiguration logger, SerilogOptions serilogOptions)
    {
        if (string.IsNullOrWhiteSpace(serilogOptions.LogPath)) return;

        const RollingInterval rollingInterval = RollingInterval.Month;
        long? fileSizeLimitBytes = 1073741824L;
        var flushToDiskInterval = new TimeSpan?();
        int? retainedFileLimitCount = 31;

        logger.WriteTo.Async(
            writeTo =>
                writeTo.File(
                    serilogOptions.LogPath,
                    0,
                    serilogOptions.LogTemplate,
                    null,
                    fileSizeLimitBytes,
                    null,
                    false,
                    false,
                    flushToDiskInterval,
                    rollingInterval,
                    false,
                    retainedFileLimitCount));
    }

    private static void SendToApplicationInsights(
        LoggerConfiguration logger,
        LogEventLevel logLevel,
        SerilogOptions options
    )
    {
        if (string.IsNullOrWhiteSpace(options.ApplicationInsightsConnectionString)) return;

        var telemetryConfig = TelemetryConfiguration.CreateDefault();
        telemetryConfig.ConnectionString = options.ApplicationInsightsConnectionString;

        logger.WriteTo.ApplicationInsights(telemetryConfig, TelemetryConverter.Traces, logLevel);
    }

    /// <summary>
    ///     configure logs to send to log analytics
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="minimumLogLevelEnum"></param>
    /// <param name="options"></param>
    private static void SendToLogAnalytics(
        LoggerConfiguration logger,
        LogEventLevel minimumLogLevelEnum,
        SerilogOptions options
    )
    {
        if (!options.LogAnalyticsLogOptions.WriteToLogAnalytics) return;

        logger.WriteTo.AzureAnalytics(
            options.LogAnalyticsLogOptions.AzureWorkspaceId,
            options.LogAnalyticsLogOptions.AzureAuthenticationId,
            new ConfigurationSettings
            {
                LogName = options.LogAnalyticsLogOptions.LogTable,
                BufferSize = options.LogAnalyticsLogOptions.BufferSize,
                BatchSize = options.LogAnalyticsLogOptions.BatchSize,
                StoreTimestampInUtc = true,
                FormatProvider = null,
            },
            minimumLogLevelEnum);
    }

    private static void WriteToElasticSearch(
        IHostEnvironment environment,
        LoggerConfiguration loggerConfiguration,
        SerilogOptions serilogOptions
    )
    {
        if (!string.IsNullOrWhiteSpace(serilogOptions.ElasticSearchUrl))
        {
            loggerConfiguration.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(serilogOptions.ElasticSearchUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                    IndexFormat =
                        $"{environment.ApplicationName}-{environment.EnvironmentName}-{DateTime.Now:yyyy-MM}",
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                });
        }
    }

    private static void WriteToGrafanaLoki(LoggerConfiguration loggerConfiguration, SerilogOptions serilogOptions)
    {
        if (!string.IsNullOrWhiteSpace(serilogOptions.GrafanaLokiUrl))
        {
            loggerConfiguration.WriteTo.GrafanaLoki(
                serilogOptions.GrafanaLokiUrl,
                new[]
                {
                    new LokiLabel {Key = "service", Value = "building-blocks",},
                },
                PropertiesAsLabels);
        }
    }

    private static void WriteToSeq(LoggerConfiguration loggerConfiguration, SerilogOptions serilogOptions)
    {
        if (!string.IsNullOrWhiteSpace(serilogOptions.SeqUrl)) loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
    }

    private static void WriteToOpenTelemetry(LoggerConfiguration loggerConfiguration, SerilogOptions serilogOptions)
    {
        if (serilogOptions.ExportLogsToOpenTelemetry) loggerConfiguration.WriteTo.OpenTelemetry();
    }
}
