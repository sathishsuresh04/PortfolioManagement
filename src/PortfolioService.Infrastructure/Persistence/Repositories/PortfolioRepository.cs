using PortfolioService.Core.Application.Data;
using PortfolioService.Core.Domain.Entities;
using PortfolioService.Core.Domain.Repositories;

namespace PortfolioService.Infrastructure.Persistence.Repositories;

public class PortfolioRepository : BaseRepository<Portfolio>, IPortfolioRepository
{
    public PortfolioRepository(IPortfolioContext context) : base(context)
    {
    }
}
