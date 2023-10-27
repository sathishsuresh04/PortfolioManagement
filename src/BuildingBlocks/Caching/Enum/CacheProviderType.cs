namespace BuildingBlocks.Caching.Enum;

/// <summary>
///     Defines the types of cache providers.
/// </summary>
public enum CacheProviderType
{
    /// <summary>
    ///     Represents an in-memory cache provider.
    /// </summary>
    InMemory = 0,

    /// <summary>
    ///     Represents a Redis cache provider.
    /// </summary>
    Redis = 1,
}
