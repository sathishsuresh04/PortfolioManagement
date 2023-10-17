namespace BuildingBlocks.Caching.Enum;

/// <summary>
///     Defines the serialization types for cache.
/// </summary>
public enum CacheSerializationType
{
    /// <summary>
    ///     Represents JSON serialization.
    /// </summary>
    Json = 0,

    /// <summary>
    ///     Represents MessagePack serialization.
    /// </summary>
    MessagePack = 1,
}
