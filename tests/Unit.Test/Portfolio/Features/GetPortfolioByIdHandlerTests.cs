using MapsterMapper;
using MongoDB.Bson;
using NSubstitute;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Dtos;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Features.GetPortfolioById;
using PortfolioService.Portfolios.Models;
using Xunit;

namespace Unit.Test.Portfolio.Features;

public class GetPortfolioByIdHandlerTests
{
    private readonly GetPortfolioByIdHandler _handler;
    private readonly IMapper _mapperSub;
    private readonly IPortfolioRepository _portfolioRepositorySub;

    public GetPortfolioByIdHandlerTests()
    {
        _portfolioRepositorySub = Substitute.For<IPortfolioRepository>();
        _mapperSub = Substitute.For<IMapper>();

        _handler = new GetPortfolioByIdHandler(_portfolioRepositorySub, _mapperSub);
    }

    [Fact]
    public async Task Handle_WithExistingPortfolioId_ShouldReturnPortfolioDto()
    {
        // Arrange
        var portfolioId = new ObjectId("61377659d24fd78398a5a54a");

        var dateTime = DateTime.UtcNow;
        var stocks = new List<Stock>
                     {
                         new() {Ticker = "TSLA", BaseCurrency = "USD", NumberOfShares = 20,},
                         new() {Ticker = "GME", BaseCurrency = "USD", NumberOfShares = 100,},
                         new() {Ticker = "KINV-B", BaseCurrency = "SEK", NumberOfShares = 50,},
                         new() {Ticker = "BBD.B", BaseCurrency = "CAD", NumberOfShares = 100,},
                         new() {Ticker = "NAS", BaseCurrency = "NOK", NumberOfShares = 20000,},
                     };

        var portfolio = PortfolioService.Portfolios.Models.Portfolio.Create(0);
        foreach (var stock in stocks) portfolio.AddStocks(stock);
        var stocksDto = new List<StockDto>
                        {
                            new("TSLA", "USD", 20),
                            new("GME", "USD", 100),
                            new("KINV-B", "SEK", 50),
                            new("BBD.B", "CAD", 100),
                            new("NAS", "NOK", 20000),
                        };

        var portfolioDto = new PortfolioDto(portfolioId.ToString(), 0, dateTime, null, null, false, stocksDto);

        _portfolioRepositorySub.GetByIdAsync(portfolioId).Returns(portfolio);
        _mapperSub.Map<PortfolioDto>(portfolio).Returns(portfolioDto);

        var request = new GetPortfolioById(portfolioId.ToString());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PortfolioDto>(result);
        Assert.Equal(portfolioDto, result);
    }

    [Fact]
    public async Task Handle_WithNonExistentPortfolioId_ShouldThrowException()
    {
        // Arrange
        var portfolioId = ObjectId.Empty;
        _portfolioRepositorySub.GetByIdAsync(portfolioId).Returns((PortfolioService.Portfolios.Models.Portfolio)null);

        var request = new GetPortfolioById(portfolioId.ToString());

        // Act and Assert
        await Assert.ThrowsAsync<PortfolioNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithNullId_ShouldThrowException()
    {
        // Arrange
        string portfolioId = null;

        var request = new GetPortfolioById(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var portfolioId = "test";

        var request = new GetPortfolioById(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidPortfolioIdException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
