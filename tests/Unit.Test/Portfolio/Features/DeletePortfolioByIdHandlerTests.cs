using MongoDB.Bson;
using NSubstitute;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Features.DeletePortfolioById;
using Xunit;

namespace Unit.Test.Portfolio.Features;



public class DeletePortfolioByIdHandlerTests
{
    private readonly DeletePortfolioByIdHandler _handler;
    private readonly IPortfolioRepository _portfolioRepository;

    public DeletePortfolioByIdHandlerTests()
    {
        _portfolioRepository = Substitute.For<IPortfolioRepository>();
        _handler = new DeletePortfolioByIdHandler(_portfolioRepository);
    }

    [Fact]
    public async Task Handle_GivenValidId_ShouldDeletePortfolio()
    {
        // Arrange
        var portfolioId = new ObjectId("61377659d24fd78398a5a54a");
        var portfolio = PortfolioService.Portfolios.Models.Portfolio.Create(0);
        _portfolioRepository.GetByIdAsync(portfolioId).Returns(portfolio);

        var request = new DeletePortfolioById(portfolioId.ToString());

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _portfolioRepository.Received(1).Update(Arg.Is<PortfolioService.Portfolios.Models.Portfolio>(x => x.Deleted == true));
        await _portfolioRepository.UnitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_GivenNonExistentPortfolioId_ShouldThrowException()
    {
        // Arrange
        var portfolioId = ObjectId.Empty;
        _portfolioRepository.GetByIdAsync(portfolioId).Returns((PortfolioService.Portfolios.Models.Portfolio)null);

        var request = new DeletePortfolioById(portfolioId.ToString());

        // Act and Assert
        await Assert.ThrowsAsync<PortfolioNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GivenNullId_ShouldThrowException()
    {
        // Arrange
        string portfolioId = null;

        var request = new DeletePortfolioById(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_GivenInvalidId_ShouldThrowException()
    {
        // Arrange
        string portfolioId = "test";

        var request = new DeletePortfolioById(portfolioId);

        // Act and Assert
        await Assert.ThrowsAsync<InvalidPortfolioIdException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
