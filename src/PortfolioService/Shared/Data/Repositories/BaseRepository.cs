using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using PortfolioService.Shared.Attributes;
using PortfolioService.Shared.Core.Primitives;
using PortfolioService.Shared.Data.Abstractions;

namespace PortfolioService.Shared.Data.Repositories;

public class BaseRepository<TEntity> : IRepository<TEntity>
where TEntity : Entity
{
    private readonly IPortfolioContext _context;
    private readonly IMongoCollection<TEntity> _dbSet;

    public BaseRepository(IPortfolioContext context)
    {
        _context = context;
        _dbSet = _context.GetCollection<TEntity>(GetCollectionName(typeof(TEntity)));
    }

    public IUnitOfWork UnitOfWork => _context;

    public void Dispose()
    {
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Get all documents present in the collection
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        var all = await _dbSet.FindAsync(Builders<TEntity>.Filter.Empty);
        return all.ToList();
    }

    /// <summary>
    ///     Get by Id (Object Id of the document)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<TEntity> GetByIdAsync(ObjectId id)
    {
        // var idFilter = Builders<PortfolioData>.Filter.Eq(portfolio => portfolio.Id, id);
        //
        // return await _portfolioCollection.Find(idFilter).FirstOrDefaultAsync();

        var data = await _dbSet.FindAsync(Builders<TEntity>.Filter.Eq("_id", id));
        return data.SingleOrDefault();
    }

    /// <summary>
    ///     Get list of documents by array of object Ids
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<ObjectId> ids)
    {
        var enumerable = ids.ToArray();
        var filter = Builders<TEntity>.Filter.In("_id", enumerable);
        var data = await _dbSet.FindAsync(filter);
        return data.ToList();
    }

    /// <summary>
    ///     Add document to collection
    /// </summary>
    /// <param name="obj"></param>
    public void Add(TEntity obj)
    {
        _context.AddCommand(() => _dbSet.InsertOneAsync(obj));
    }

    /// <summary>
    ///     Update document to collection
    /// </summary>
    /// <param name="obj"></param>
    public void Update(TEntity obj)
    {
        _context.AddCommand(
            () => _dbSet.FindOneAndReplaceAsync(
                Builders<TEntity>.Filter.Eq("_id", obj.Id),
                obj,
                new FindOneAndReplaceOptions<TEntity> {ReturnDocument = ReturnDocument.After,}));
    }

    // Context.AddCommand(() => DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq("_id", obj.GetId()), obj));
    /// <summary>
    ///     Remove document by object Id
    /// </summary>
    /// <param name="id"></param>
    public void Remove(ObjectId id)
    {
        _context.AddCommand(() => _dbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq("_id", id)));
    }

    /// <summary>
    ///     Check collection exists, if not create a new collection
    /// </summary>
    /// <param name="collectionName"></param>
    private void CheckAndCreateCollection(string collectionName)
    {
        if (!_context.GetDatabase().ListCollectionNames().ToList().Contains(collectionName))
        {
            _context.GetDatabase()
                .CreateCollection(
                    collectionName,
                    new CreateCollectionOptions
                    {
                        Capped = false, Collation = new Collation("sv", strength: CollationStrength.Primary),
                    });
        }
    }

    /// <summary>
    ///     Get collection name from mongo db database
    /// </summary>
    /// <param name="documentType"></param>
    /// <returns></returns>
    private static string GetCollectionName(ICustomAttributeProvider documentType)
    {
        return ((BsonCollectionAttribute)documentType.GetCustomAttributes(
                           typeof(BsonCollectionAttribute),
                           true)
                       .FirstOrDefault())?.CollectionName;
    }
}
