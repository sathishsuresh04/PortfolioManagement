using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Logging;

/// <summary>
///     Represents the options for Serilog configuration.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public sealed class SerilogOptions
{
    /// <summary>
    ///     Gets or sets the log template used by Serilog.
    /// </summary>
    public string LogTemplate { get; init; } =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level} - {Message:lj}{NewLine}{Exception}";

    /// <summary>
    ///     Gets or sets the minimum log level for filtering log messages.
    /// </summary>
    public string? MinimumLogLevel { get; init; }

    /// <summary>
    ///     Gets or sets the name of the service.
    /// </summary>
    public string ServiceName { get; init; } = null!;

    /// <summary>
    ///     Gets or sets the options for console logging.
    /// </summary>
    public ConsoleLogOptions ConsoleLogOptions { get; init; } = null!;

    /// <summary>
    ///     Gets or sets the log path for file logging.
    /// </summary>
    public string? LogPath { get; init; }

    /// <summary>
    ///     Gets or sets the Application Insights connection string.
    /// </summary>
    public string? ApplicationInsightsConnectionString { get; init; }

    /// <summary>
    ///     Gets or sets the options for logging to Azure Log Analytics.
    /// </summary>
    public LogAnalyticsLogOptions LogAnalyticsLogOptions { get; init; } = null!;

    /// <summary>
    ///     Gets or sets the URL for Elasticsearch logging.
    /// </summary>
    public string? ElasticSearchUrl { get; init; }

    /// <summary>
    ///     Gets or sets the URL for Grafana Loki logging.
    /// </summary>
    public string? GrafanaLokiUrl { get; init; }

    /// <summary>
    ///     Gets or sets the URL for Seq logging.
    /// </summary>
    public string? SeqUrl { get; init; }

    /// <summary>
    ///     Gets or sets a value indicating whether to export logs to OpenTelemetry.
    /// </summary>
    public bool ExportLogsToOpenTelemetry { get; init; }
}

/// <summary>
///     Represents the options for console logging.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record ConsoleLogOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to write log messages to the console.
    /// </summary>
    public bool WriteToConsole { get; init; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether to use Elasticsearch JSON formatter for console logs.
    /// </summary>
    public bool UseElasticsearchJsonFormatter { get; init; }
}

/// <summary>
///     Represents the options for logging to Azure Log Analytics.
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record LogAnalyticsLogOptions
{
    /// <summary>
    ///     Gets or sets a value indicating whether to write log messages to Azure Log Analytics.
    /// </summary>
    public bool WriteToLogAnalytics { get; init; }

    /// <summary>
    ///     Gets or sets the Azure workspace ID for Azure Log Analytics.
    /// </summary>
    public string? AzureWorkspaceId { get; init; }

    /// <summary>
    ///     Gets or sets the Azure authentication ID for Azure Log Analytics.
    /// </summary>
    public string? AzureAuthenticationId { get; init; }

    /// <summary>
    ///     Gets or sets the log table for Azure Log Analytics.
    /// </summary>
    public string? LogTable { get; init; }

    /// <summary>
    ///     Gets or sets the buffer size for batching log messages to Azure Log Analytics.
    /// </summary>
    public int BufferSize { get; init; }

    /// <summary>
    ///     Gets or sets the batch size for sending log messages to Azure Log Analytics.
    /// </summary>
    public int BatchSize { get; init; }
}
