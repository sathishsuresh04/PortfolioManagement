using MongoDB.Bson;
using PortfolioService.Shared.Core.Primitives;

namespace PortfolioService.Shared.Data.Abstractions;

public interface IRepository<TEntity> : IDisposable
where TEntity : Entity
{
    IUnitOfWork UnitOfWork { get; }
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity> GetByIdAsync(ObjectId id);
    void Add(TEntity obj);
    void Update(TEntity obj);
    void Remove(ObjectId id);
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<ObjectId> ids);
}
