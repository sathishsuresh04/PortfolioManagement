using System.Security.Authentication;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PortfolioService.Shared.Data.Abstractions;
using PortfolioService.Shared.Options;

namespace PortfolioService.Shared.Data;

public class PortfolioContext : IPortfolioContext
{
    private readonly List<Func<Task>> _commands;
    private readonly MongoDbOptions _databaseSettings;

    public PortfolioContext(IOptions<MongoDbOptions> options)
    {
        _databaseSettings = options.Value;
        ArgumentNullException.ThrowIfNull(_databaseSettings, nameof(MongoDbOptions));
        ConfigureMongo();
        _commands = new List<Func<Task>>();
    }

    private IMongoDatabase Database { get; set; }
    private IClientSessionHandle Session { get; set; }
    private MongoClient MongoClient { get; set; }

    /// <summary>
    ///     Get Mongo client incase needed for extending the functionality.
    /// </summary>
    /// <returns></returns>
    public MongoClient GetClient()
    {
        return MongoClient;
    }

    public IMongoDatabase GetDatabase()
    {
        return Database;
    }

    /// <summary>
    ///     Get the collection by name
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return Database.GetCollection<T>(name);
    }


    /// <summary>
    ///     Add commands to save
    /// </summary>
    /// <param name="func"></param>
    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }


    /// <summary>
    ///     Dispose object
    /// </summary>
    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Save the changes in the database as part of uow
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using (Session = await MongoClient.StartSessionAsync(cancellationToken: cancellationToken))
        {
            Session.StartTransaction();

            var commandTasks = _commands.Select(c => c());

            await Task.WhenAll(commandTasks);

            await Session.CommitTransactionAsync(cancellationToken);
        }

        return _commands.Count;
    }

    /// <summary>
    ///     Configure Mongo client setup
    /// </summary>
    private void ConfigureMongo()
    {
        if (MongoClient != null) return;

        var settings = MongoClientSettings.FromConnectionString(_databaseSettings.ConnectionString);
        settings.SslSettings = new SslSettings {EnabledSslProtocols = SslProtocols.Tls12,};
        MongoClient = new MongoClient(settings);
        // Configure mongo (You can inject the config, just to simplify)
        // MongoClient = new MongoClient(_databaseSettings.ConnectionString);

        Database = MongoClient.GetDatabase(_databaseSettings.DatabaseName);
    }
}
