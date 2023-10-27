using Ardalis.GuardClauses;
using PortfolioService.Shared.Attributes;
using PortfolioService.Shared.Core.Abstractions;
using PortfolioService.Shared.Core.Primitives;

namespace PortfolioService.Portfolios.Models;

[BsonCollection(Constants.MongoCollectionNames.Portfolios)]
public class Portfolio : AggregateRoot, IAuditableEntity, ISoftDeletableEntity
{
    private List<Stock> _stocks = null!;

    public ICollection<Stock> Stocks
    {
        get => _stocks ??= new List<Stock>();
        set => _stocks = new List<Stock>(value);
    }

    public float CurrentTotalValue { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }
    public DateTime? DeletedOnUtc { get; private set; }
    public bool Deleted { get; private set; }

    public static Portfolio Create(float currentTotalValue)
    {
        return new Portfolio {CurrentTotalValue = currentTotalValue,};
    }


    public void SetCreationDate(DateTime dateTime)
    {
        CreatedOnUtc = dateTime;
    }

    public void SetModifiedDate(DateTime dateTime)
    {
        ModifiedOnUtc = dateTime;
    }

    public void SetSoftDelete(DateTime dateTime)
    {
        ModifiedOnUtc = dateTime;
        DeletedOnUtc = dateTime;
        Deleted = true;
    }

    public void AddStocks(Stock stock)
    {
        Guard.Against.Null(stock);
        Stocks.Add(stock);
    }
}
