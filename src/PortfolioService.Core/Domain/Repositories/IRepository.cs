using PortfolioService.Core.Application.Data;
using PortfolioService.Core.Domain.Core.Primitives;

namespace PortfolioService.Core.Domain.Repositories;

public interface IRepository<TEntity> : IDisposable
where TEntity : Entity
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(string id);
    void Add(TEntity obj);
    void Update(TEntity obj);
    void Remove(Guid id);
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<string> ids);
}
