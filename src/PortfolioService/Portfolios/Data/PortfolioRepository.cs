using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Models;
using PortfolioService.Shared.Data.Abstractions;
using PortfolioService.Shared.Data.Repositories;

namespace PortfolioService.Portfolios.Data;

public class PortfolioRepository : BaseRepository<Portfolio>, IPortfolioRepository
{
    public PortfolioRepository(IPortfolioContext context) : base(context)
    {
    }
}
