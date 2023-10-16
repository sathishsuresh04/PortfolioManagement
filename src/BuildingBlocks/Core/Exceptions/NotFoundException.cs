using System.Net;

namespace BuildingBlocks.Core.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message)
        : base(message)
    {
        StatusCode = HttpStatusCode.NotFound;
    }
}
