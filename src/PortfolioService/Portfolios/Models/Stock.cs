namespace PortfolioService.Portfolios.Models;

public class Stock
{
    public string Ticker { get;  set; } = default!;
    public string BaseCurrency { get;  set; } = default!;
    public int NumberOfShares { get;  set; }

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
