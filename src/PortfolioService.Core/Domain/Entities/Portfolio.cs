using Ardalis.GuardClauses;
using PortfolioService.Core.Constants;
using PortfolioService.Core.Domain.Attributes;
using PortfolioService.Core.Domain.Core.Abstractions;
using PortfolioService.Core.Domain.Core.Primitives;

namespace PortfolioService.Core.Domain.Entities;

[BsonCollection(PortfolioConstants.MongoCollectionNames.Portfolios)]
public class Portfolio : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
{
    private readonly List<Stock> _stocks = new();

    public float CurrentTotalValue { get; private set; }
    public ICollection<Stock> Stocks => _stocks.AsReadOnly();

    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; }
    public DateTime? DeletedOnUtc { get; }
    public bool Deleted { get; }

    public static Portfolio Create(float currentTotalValue)
    {
        return new Portfolio {CurrentTotalValue = currentTotalValue,};
    }

    public void AddStocks(Stock stock)
    {
        Guard.Against.Null(stock);
        _stocks.Add(stock);
    }
}
