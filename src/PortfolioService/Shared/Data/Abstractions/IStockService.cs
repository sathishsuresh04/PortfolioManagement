namespace PortfolioService.Shared.Abstractions;

public interface IStockService
{
    Task<(decimal Price, string BaseCurrency)> GetCurrentStockPrice(string ticker);
}
