namespace BuildingBlocks.Abstractions.Persistence;

public interface IDataSeeder
{
    /// <summary>Seeds all the initial data asynchronously.</summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SeedAllAsync();
}
