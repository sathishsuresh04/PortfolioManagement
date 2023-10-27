using BuildingBlocks.Caching.Enum;

namespace BuildingBlocks.Caching;

public class CacheOptions
{
    /// <summary>
    ///     Gets or sets the default cache type.
    /// </summary>
    public string DefaultCacheType { get; set; } = nameof(CacheProviderType.InMemory);

    /// <summary>
    ///     Gets or sets the expiration time in minutes for cache entries.
    /// </summary>
    public double ExpirationTimeInMinute { get; set; } = 5;

    /// <summary>
    ///     Gets or sets the serialization type for cache entries.
    /// </summary>
    public string SerializationType { get; set; } = nameof(CacheSerializationType.Json);

    /// <summary>
    ///     Gets or sets the Redis cache options.
    /// </summary>
    public RedisCacheOptions? RedisCacheOptions { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the in-memory cache options.
    /// </summary>
    public InMemoryCacheOptions? InMemoryOptions { get; set; } = default!;

    /// <summary>
    ///     Gets or sets the default cache prefix.
    /// </summary>
    public string DefaultCachePrefix { get; set; } = "Ch_";
}

/// <summary>
///     Options for Redis cache configuration.
/// </summary>
public class RedisCacheOptions
{
    /// <summary>
    ///     Gets or sets the connection string for Redis cache.
    /// </summary>
    public string ConnectionString { get; set; } = default!;
}

/// <summary>
///     Options for in-memory cache configuration.
/// </summary>
public class InMemoryCacheOptions
{
    // No additional properties are needed for in-memory cache options.
}
