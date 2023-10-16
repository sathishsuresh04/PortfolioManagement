using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioService.Core.Application.Data;

public interface IUnitOfWork: IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}