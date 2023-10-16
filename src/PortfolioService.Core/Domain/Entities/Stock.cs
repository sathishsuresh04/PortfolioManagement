namespace PortfolioService.Core.Domain.Entities;

public class Stock
{
    public string Ticker { get; private set; } = default!;
    public string BaseCurrency { get; private set; } = default!;
    public int NumberOfShares { get; private set; }

    /// <summary>
    ///     Create new stock
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="baseCurrency"></param>
    /// <param name="numberOfShares"></param>
    /// <returns></returns>
    public static Stock Create(string ticker, string baseCurrency, int numberOfShares)
    {
        var stock = new Stock {Ticker = ticker, BaseCurrency = baseCurrency, NumberOfShares = numberOfShares,};
        return stock;
    }
}
