using System.Net;

namespace BuildingBlocks.Core.Exceptions;

public class ValidationException : CustomException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ValidationException" /> class with the specified message, inner
    ///     exception, and error messages.
    /// </summary>
    /// <param name="message">The error message that describes the validation error.</param>
    /// <param name="statusCode"></param>
    public ValidationException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
