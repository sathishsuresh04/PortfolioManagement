using MongoDB.Bson;
using PortfolioService.Portfolios.Exceptions;

namespace PortfolioService.Portfolios.Models.ValueObjects;

public record PortfolioId
{
    private PortfolioId(ObjectId value)
    {
        Value = value;
    }

    public ObjectId Value { get; }

    public static PortfolioId CreateNew()
    {
        var objectId = ObjectId.GenerateNewId();
        return new PortfolioId(objectId);
    }

    public static PortfolioId Of(string value)
    {
        if (!ObjectId.TryParse(value, out var objectId)) throw new InvalidPortfolioIdException(value);
        return new PortfolioId(objectId);
    }

    public static implicit operator ObjectId(PortfolioId portfolioId)
    {
        return portfolioId.Value;
    }
}
