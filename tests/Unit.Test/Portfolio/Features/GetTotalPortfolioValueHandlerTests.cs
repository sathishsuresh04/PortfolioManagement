using BuildingBlocks.Caching.Enum;
using EasyCaching.Core;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using NSubstitute;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Features.GetTotalPortfolioValue;
using PortfolioService.Portfolios.Models;
using PortfolioService.Shared.Data.Abstractions;
using PortfolioService.Shared.Data.Services;
using PortfolioService.Shared.Dtos;
using PortfolioService.Shared.Options;
using Xunit;

namespace Unit.Test.Portfolio.Features;

public class GetTotalPortfolioValueHandlerTests
{
    private readonly GetTotalPortfolioValueHandler _handler;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly ExchangeRateService _exchangeRateService;
    private readonly IStockService _stockService;
    private readonly IEasyCachingProvider _mockedCachingProvider;
    private const string CacheKey = "exchange_rate_data";

    public GetTotalPortfolioValueHandlerTests()
    {
        _mockedCachingProvider = Substitute.For<IEasyCachingProvider>();
        var mockedCachingProviderFactory = Substitute.For<IEasyCachingProviderFactory>();
        _portfolioRepository = Substitute.For<IPortfolioRepository>();
        _stockService = Substitute.For<IStockService>();
        mockedCachingProviderFactory.GetCachingProvider(nameof(CacheProviderType.InMemory))
            .Returns(_mockedCachingProvider);
        var exchangeRateApiClient = Substitute.For<IExchangeRateApiClient>();
        var mockedExchangeRateApiOptions = new ExchangeRateApiOptions();
        var optionsWrapper = Substitute.For<IOptions<ExchangeRateApiOptions>>();
        optionsWrapper.Value.Returns(mockedExchangeRateApiOptions);
        _exchangeRateService = new ExchangeRateService(mockedCachingProviderFactory, exchangeRateApiClient, optionsWrapper);
        _handler = new GetTotalPortfolioValueHandler(_portfolioRepository, _stockService, _exchangeRateService);
    }

    [Fact]
    public async Task Handle_GivenValidId_ShouldCalculateTotalPortfolioValue()
    {
        // Arrange
        var portfolioId = new ObjectId("61377659d24fd78398a5a54a");
        var portfolio = PortfolioService.Portfolios.Models.Portfolio.Create(0);
        var stocks = new List<Stock>
                     {
                         new() {Ticker = "TSLA", BaseCurrency = "USD", NumberOfShares = 10},
                     };

        foreach (var stock in stocks)
        {
            portfolio.AddStocks(stock);
        }

        _portfolioRepository.GetByIdAsync(portfolioId).Returns(portfolio);

        var data = new Quote { quotes = new Dictionary<string, decimal> { { "USDUSD", 1m } } };

        // Mocking the GetAsync method to return our quote data
        var cacheValue = new CacheValue<Quote>(data, true);
        _mockedCachingProvider.GetAsync<Quote>(CacheKey, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(cacheValue));

        _exchangeRateService.GetExchangeRateAsync(Arg.Any<CancellationToken>()).Returns(data);
        _stockService.GetCurrentStockPrice(Arg.Any<string>()).Returns((640, "USD"));
        var request = new GetTotalPortfolioValue(portfolioId.ToString());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Check if the method was called
        _mockedCachingProvider.Received().GetAsync<Quote>(CacheKey, Arg.Any<CancellationToken>());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<GetTotalPortfolioValueResponse>(result);

        Assert.Equal(6400, result.TotalValue);
    }


    [Fact]
    public async Task Handle_GivenNonExistentPortfolioId_ShouldThrowException()
    {
        // Arrange
        var portfolioId = ObjectId.Empty;
        _portfolioRepository.GetByIdAsync(portfolioId).Returns((PortfolioService.Portfolios.Models.Portfolio)null);

        var request = new GetTotalPortfolioValue(portfolioId.ToString());

        // Act and Assert
        await Assert.ThrowsAsync<PortfolioNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GivenNullId_ShouldThrowException()
    {
        // Arrange
        string portfolioId = null;

        var request = new GetTotalPortfolioValue(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GivenInvalidId_ShouldThrowException()
    {
        // Arrange
        string portfolioId = "test";

        var request = new GetTotalPortfolioValue(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidPortfolioIdException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
