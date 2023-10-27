using BuildingBlocks.Core.Exceptions;

namespace PortfolioService.Portfolios.Exceptions;

public class PortfolioNotFoundException : NotFoundException
{
    public PortfolioNotFoundException(string portfolioId)
        : base($"Portfolio with Id {portfolioId} not found.")
    {
    }
}
