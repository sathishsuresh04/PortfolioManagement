using MongoDB.Driver;

namespace PortfolioService.Shared.Data.Abstractions;

public interface IPortfolioContext : IUnitOfWork
{
    MongoClient GetClient();
    IMongoDatabase GetDatabase();
    IMongoCollection<T> GetCollection<T>(string name);
    void AddCommand(Func<Task> func);
}
