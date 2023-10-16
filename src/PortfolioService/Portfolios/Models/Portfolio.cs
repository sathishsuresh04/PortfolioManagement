using Ardalis.GuardClauses;
using PortfolioService.Shared.Attributes;
using PortfolioService.Shared.Core.Abstractions;
using PortfolioService.Shared.Core.Primitives;

namespace PortfolioService.Portfolios.Models;

[BsonCollection(Constants.MongoCollectionNames.Portfolios)]
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
