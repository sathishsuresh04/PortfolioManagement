namespace PortfolioService.Shared.Data.Abstractions;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
