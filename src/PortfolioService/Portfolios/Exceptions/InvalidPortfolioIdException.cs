using System.Net;
using BuildingBlocks.Core.Exceptions;
using MongoDB.Bson;

namespace PortfolioService.Portfolios.Exceptions;

#pragma warning disable S3925
public class InvalidPortfolioIdException : DomainException
#pragma warning restore S3925
{
    public InvalidPortfolioIdException(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest) :
        base(message, statusCode)
    {
    }

    public InvalidPortfolioIdException(ObjectId portfolioId)
        : base($"portfolioId: '{portfolioId}' is invalid.")
    {
    }
}
