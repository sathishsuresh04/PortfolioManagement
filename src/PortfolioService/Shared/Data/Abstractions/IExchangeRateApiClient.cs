using PortfolioService.Shared.Dtos;
using Refit;

namespace PortfolioService.Shared.Data.Abstractions;

public interface IExchangeRateApiClient
{
    [Get("/live?access_key={apiAccessKey}")]
    Task<Quote> GetExchangeRateAsync(string apiAccessKey);
}
