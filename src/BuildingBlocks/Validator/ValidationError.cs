namespace BuildingBlocks.Validator;

/// <summary>
///     Represents a validation error with a field and error message.
/// </summary>
public class ValidationError
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationError" /> class.
    /// </summary>
    /// <param name="field">The field associated with the error.</param>
    /// <param name="message">The error message.</param>
    public ValidationError(string field, string message)
    {
        Field = field != string.Empty ? field : null;
        Message = message;
    }

    /// <summary>
    ///     Gets the field associated with the error.
    /// </summary>
    public string? Field { get; }

    /// <summary>
    ///     Gets the error message.
    /// </summary>
    public string Message { get; }
}
