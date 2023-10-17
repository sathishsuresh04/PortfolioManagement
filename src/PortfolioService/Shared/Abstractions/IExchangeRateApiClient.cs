using PortfolioService.Shared.Dtos;
using Refit;

namespace PortfolioService.Shared.Abstractions;

public interface IExchangeRateApiClient
{
    [Get("live?access_key={apiAccessKey}")]
    Task<Quote> GetExchangeRateAsync(string apiAccessKey);
}
