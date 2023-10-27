using System.Text.Json;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Validator;

/// <summary>
///     Represents the model for validation results.
/// </summary>
public class ValidationResultModel
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationResultModel" /> class.
    /// </summary>
    /// <param name="validationResult">The validation result.</param>
    public ValidationResultModel(ValidationResult? validationResult = null)
    {
        Errors = validationResult?.Errors
            .Select(error => new ValidationError(error.PropertyName, error.ErrorMessage))
            .ToList();
        Message = JsonSerializer.Serialize(Errors);
    }

    /// <summary>
    ///     Gets or sets the status code associated with the validation result.
    /// </summary>
    public int StatusCode { get; set; } = StatusCodes.Status400BadRequest;

    /// <summary>
    ///     Gets or sets the error message associated with the validation result.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    ///     Gets the list of validation errors.
    /// </summary>
    public IList<ValidationError>? Errors { get; }

    /// <summary>
    ///     Converts the object to its JSON representation.
    /// </summary>
    /// <returns>The JSON representation of the object.</returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
