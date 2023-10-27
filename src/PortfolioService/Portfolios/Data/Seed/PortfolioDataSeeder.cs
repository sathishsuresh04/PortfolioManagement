using BuildingBlocks.Abstractions.Persistence;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Models;

namespace PortfolioService.Portfolios.Data.Seed;

public class PortfolioDataSeeder : IDataSeeder
{
    private readonly IPortfolioRepository _portfolioRepository;

    public PortfolioDataSeeder(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
    }

    public async Task SeedAllAsync()
    {
        var existingPortfolios = await _portfolioRepository.GetAllAsync();
        if (!existingPortfolios.Any())
        {
            CreatePortfolio(
                new List<Stock>
                {
                    Stock.Create("TSLA", "USD", 20),
                    Stock.Create("GME", "USD", 100),
                    Stock.Create("KINV-B", "SEK", 50),
                    Stock.Create("BBD.B", "CAD", 100),
                    Stock.Create("NAS", "NOK", 20000),
                });

            CreatePortfolio(
                new List<Stock>
                {
                    Stock.Create("TSLA", "USD", 1),
                    Stock.Create("GME", "USD", 3457),
                    Stock.Create("KINV-B", "SEK", 3457),
                    Stock.Create("BBD.B", "CAD", 5768),
                    Stock.Create("NAS", "NOK", 100000),
                });

            await _portfolioRepository.UnitOfWork.SaveChangesAsync(new CancellationToken());
        }
    }

    private void CreatePortfolio(IEnumerable<Stock> stocksList)
    {
        var portfolio = Portfolio.Create(0);
        portfolio.SetCreationDate(DateTime.UtcNow); //use machine datetime service
        foreach (var stock in stocksList) portfolio.AddStocks(stock);
        _portfolioRepository.Add(portfolio);
    }
}
