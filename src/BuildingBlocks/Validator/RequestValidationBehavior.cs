using System.Text.Json;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Validator;

/// <summary>
///     A behavior that performs request validation using FluentValidation.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
where TResponse : class
{
    private readonly ILogger<RequestValidationBehavior<TRequest, TResponse>> _logger;
    private readonly IServiceProvider _serviceProvider;

    public RequestValidationBehavior(
        IServiceProvider serviceProvider,
        ILogger<RequestValidationBehavior<TRequest, TResponse>> logger
    )
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var requestType = typeof(TRequest);
        var responseType = typeof(TResponse);

        object objectToValidate = request;
        var validator = _serviceProvider.GetService<IValidator<TRequest>>()!;
        if (validator is null) return await next();

        _logger.LogInformation(
            "[{Prefix}] Handle request={RequestData} and response={ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            typeof(TRequest).Name,
            typeof(TResponse).Name);

        _logger.LogInformation(
            "[{Prefix}] Handle request={X-RequestData} and response={X-ResponseData}",
            nameof(RequestValidationBehavior<TRequest, TResponse>),
            requestType.Name,
            responseType.Name);

        _logger.LogDebug(
            "Handling {FullName} with content {Request}",
            requestType.FullName,
            JsonSerializer.Serialize(request));

        var validationContext = new ValidationContext<object>(objectToValidate);
        var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ToValidationResultModel().Message);

        var response = await next();

        _logger.LogInformation("Handled {FullName}", requestType.FullName);
        return response;
    }
}
