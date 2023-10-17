using BuildingBlocks.Caching.Enum;
using EasyCaching.Core;
using Microsoft.Extensions.Options;
using PortfolioService.Shared.Data.Abstractions;
using PortfolioService.Shared.Dtos;
using PortfolioService.Shared.Options;
using Refit;

namespace PortfolioService.Shared.Infrastructure;

public class ExchangeRateService
{
    private const int CacheHours = 24;
    private readonly IEasyCachingProvider _cachingProvider;
    private readonly IExchangeRateApiClient _exchangeRateApiClient;
    private const string CacheKey = "exchange_rate_data";
    private readonly ExchangeRateApiOptions _exchangeRateApiOptions;

    public ExchangeRateService(
        IEasyCachingProviderFactory cachingProviderFactory,
        IExchangeRateApiClient exchangeRateApiClient,
        IOptions<ExchangeRateApiOptions> options
    )
    {
        _cachingProvider = cachingProviderFactory.GetCachingProvider(nameof(CacheProviderType.InMemory));
        _exchangeRateApiClient = exchangeRateApiClient;
        _exchangeRateApiOptions = options.Value;
    }

    public async Task<Quote> GetExchangeRateAsync(CancellationToken cancellationToken)
    {
        var cacheValue = await _cachingProvider.GetAsync<Quote>(CacheKey,cancellationToken);
        if (cacheValue.HasValue)
        {
            return cacheValue.Value;
        }

        try
        {
            var data = await _exchangeRateApiClient.GetExchangeRateAsync(_exchangeRateApiOptions.Token);
            if (data != null)
            {
                // cache the data and set an expiration time of 24 hours
                await _cachingProvider.SetAsync(CacheKey, data, TimeSpan.FromHours(CacheHours), cancellationToken);
            }

            return data;
        }
        catch (ApiException ex)
        {
            throw new HttpRequestException(
                $"Error occurred while retrieving exchange rate data. Status Code: {ex.StatusCode}. Reason:{ex.Content}", ex);
        }

    }
}
