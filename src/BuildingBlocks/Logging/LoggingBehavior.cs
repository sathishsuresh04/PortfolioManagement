using System.Diagnostics;
using BuildingBlocks.Abstractions.Serialization;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace BuildingBlocks.Logging;

/// <summary>
///     Pipeline behavior that logs information before and after handling a request,
///     and logs a warning if the request took longer than 3 seconds to process.
/// </summary>
/// <typeparam name="TRequest">Type of the request</typeparam>
/// <typeparam name="TResponse">Type of the response</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : class
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ISerializer _serializer;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ISerializer serializer)
    {
        _logger = logger;
        _serializer = serializer;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        using (LogContext.PushProperty("RequestObject", _serializer.Serialize(request)))
        {
            var requestType = typeof(TRequest);
            var responseType = typeof(TResponse);

            const string prefix = nameof(LoggingBehavior<TRequest, TResponse>);

            _logger.LogInformation(
                "[{Prefix}] Handle request '{RequestData}' and response '{ResponseData}'",
                prefix,
                requestType.Name,
                responseType.Name);

            var timer = new Stopwatch();
            timer.Start();

            var response = await next();

            timer.Stop();
            var timeTaken = timer.Elapsed;
            if (timeTaken.Seconds > 3)
            {
                _logger.LogWarning(
                    "[{PerfPossible}] The request '{RequestData}' took '{TimeTaken}' seconds",
                    prefix,
                    requestType.Name,
                    timeTaken.Seconds);
            }
            else
            {
                _logger.LogInformation(
                    "[{PerfPossible}] The request '{RequestData}' took '{TimeTaken}' seconds",
                    prefix,
                    requestType.Name,
                    timeTaken.Seconds);
            }

            _logger.LogInformation("[{Prefix}] Handled '{RequestData}'", prefix, requestType.Name);

            return response;
        }
    }
}
