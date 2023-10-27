using System.Net;

namespace BuildingBlocks.Core.Exceptions;

public class DomainException : CustomException
{
    public DomainException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
