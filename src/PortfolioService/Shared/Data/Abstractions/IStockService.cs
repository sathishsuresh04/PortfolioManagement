namespace PortfolioService.Shared.Data.Abstractions;

public interface IStockService
{
    Task<(decimal Price, string BaseCurrency)> GetCurrentStockPrice(string ticker);
}
