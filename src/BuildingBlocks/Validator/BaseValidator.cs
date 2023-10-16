using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;

namespace BuildingBlocks.Validator;

/// <summary>
///     Abstract base class for validators that includes additional functionality for validating null values.
/// </summary>
/// <typeparam name="T">The type of object being validated.</typeparam>
public abstract class BaseValidator<T> : AbstractValidator<T>
{
    /// <summary>
    ///     Overrides the PreValidate method to ensure that the instance to validate is not null, and if it is,
    ///     adds an error to the ValidationResult indicating that it cannot be null.
    /// </summary>
    /// <param name="context">The validation context.</param>
    /// <param name="result">The validation result.</param>
    /// <returns>A boolean indicating whether the validation was successful.</returns>
    protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
    {
        if (context.InstanceToValidate != null) return base.PreValidate(context, result);
        var name = typeof(T).Name;
        ((ICollection<ValidationFailure>)result.Errors).Add(new ValidationFailure("", name + " cannot be null"));
        return false;
    }

    /// <summary>
    ///     Adds a rule for the specified property and cascade mode to the validator.
    /// </summary>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <param name="cascadeMode">The cascade mode to use for the validation rule.</param>
    /// <returns>The rule builder.</returns>
    public IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(
        Expression<Func<T, TProperty>> expression,
        CascadeMode cascadeMode
    )
    {
        return RuleFor(expression).Cascade(cascadeMode);
    }
}
