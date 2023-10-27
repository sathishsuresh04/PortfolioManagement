using PortfolioService.Portfolios.Models;
using PortfolioService.Shared.Data.Abstractions;

namespace PortfolioService.Portfolios.Data.Abstractions;

public interface IPortfolioRepository : IRepository<Portfolio>
{
}
