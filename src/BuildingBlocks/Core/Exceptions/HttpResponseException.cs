using System.Net;

namespace BuildingBlocks.Core.Exceptions;

public class HttpResponseException : CustomException
{
    public HttpResponseException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message, statusCode)
    {
    }
}
