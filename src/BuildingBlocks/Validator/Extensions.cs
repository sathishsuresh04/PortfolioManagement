using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using Scrutor;

namespace BuildingBlocks.Validator;

public static class Extension
{
    /// <summary>
    ///     Adds custom validators from the specified assembly to the service collection1.
    /// </summary>
    /// <param name="services">The service collection to add the validators to.</param>
    /// <param name="assembly">The assembly containing the validators.</param>
    /// <param name="serviceLifetime">The lifetime of the registered services (default: Transient).</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddCustomValidators(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient
    )
    {
        services.Scan(
            scan =>
                scan.FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithLifetime(serviceLifetime));

        return services;
    }

    /// <summary>
    ///     Handles the validation of a request using the specified validator asynchronously.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="request">The request to be validated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validated request.</returns>
    /// <exception cref="Core.Exceptions.Types.ValidationException">Thrown when the request is not valid.</exception>
    public static async Task<TRequest> HandleValidationAsync<TRequest>(
        this IValidator<TRequest> validator,
        TRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ToValidationResultModel().Message);

        return request;
    }

    /// <summary>
    ///     Handles the validation of a request using the specified validator.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <param name="validator">The validator to use for validation.</param>
    /// <param name="request">The request to be validated.</param>
    /// <returns>The validated request.</returns>
    /// <exception cref="Core.Exceptions.Types.ValidationException">Thrown when the request is not valid.</exception>
    public static TRequest HandleValidation<TRequest>(this IValidator<TRequest> validator, TRequest request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.ToValidationResultModel().Message);

        return request;
    }

    /// <summary>
    ///     Converts a FluentValidation ValidationResult to a ValidationResultModel.
    /// </summary>
    /// <param name="validationResult">The FluentValidation ValidationResult.</param>
    /// <returns>The converted ValidationResultModel.</returns>
    public static ValidationResultModel ToValidationResultModel(this ValidationResult? validationResult)
    {
        return new ValidationResultModel(validationResult);
    }

    /// <summary>
    ///     Sets the error message for all components of the rule to the specified value.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <typeparam name="TProperty">The type of the property being validated.</typeparam>
    /// <param name="rule">The rule instance to set the error message on.</param>
    /// <param name="errorMessage">The error message to set.</param>
    /// <returns>The same rule instance with the updated error message.</returns>
    public static IRuleBuilderOptions<T, TProperty> WithGlobalMessage<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule,
        string errorMessage
    )
    {
        if (rule.GetType().GetProperty("Rule")?.GetValue(rule) is not IValidationRule<T, TProperty> propertyRule)
            return rule;
        foreach (var component in propertyRule.Components.OfType<IRuleComponent<T, TProperty>>())
            component.SetErrorMessage(errorMessage);

        return rule;
    }

    /// <summary>
    ///     Validates that a string value is a valid enum name for the specified <paramref name="enumType" /> and provides a
    ///     custom error message.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="rule">The rule instance to add the validation to.</param>
    /// <param name="propertyNameProvider">A function that provides the name of the property being validated.</param>
    /// <param name="propertyValueProvider">A function that provides the value of the property being validated.</param>
    /// <param name="enumType">The type of the enum to validate against.</param>
    /// <param name="caseSensitive">A flag indicating whether the enum name should be validated in a case-sensitive manner.</param>
    /// <returns>The same rule instance with the added validation.</returns>
    public static IRuleBuilderOptions<T, string> IsValidEnum<T>(
        this IRuleBuilder<T, string> rule,
        Func<T, string> propertyNameProvider,
        Func<T, string> propertyValueProvider,
        Type enumType,
        bool caseSensitive = false
    )
    {
        return rule.NotEmpty()
            .WithMessage((Func<T, string>)(x => propertyNameProvider(x) + " cannot be null or empty"))
            .IsEnumName(enumType, caseSensitive)
            .WithMessage<T, string>(
                (Func<T, string>)(x =>
                                      propertyNameProvider(x) + " is invalid: " + propertyValueProvider(x)));
    }

    /// <summary>
    ///     Checks if a given date is a valid date.
    ///     A valid date is any date that is not the default value for DateTime and not DateTime.MinValue.
    /// </summary>
    /// <param name="date">The date to be checked.</param>
    /// <returns>Returns true if the date is valid; otherwise, false.</returns>
    public static bool BeAValidDate(DateTime date)
    {
        return date != default && date != DateTime.MinValue;
    }

    /// <summary>
    ///     Checks if a given date is not in the future.
    ///     A date is considered not in the future if it is less than or equal to the current UTC date/time.
    /// </summary>
    /// <param name="date">The date to be checked.</param>
    /// <returns>Returns true if the date is not in the future; otherwise, false.</returns>
    public static bool NotBeAFutureDate(DateTime date)
    {
        return date <= DateTime.UtcNow;
    }
}
