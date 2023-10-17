using Ardalis.GuardClauses;
using BuildingBlocks.Caching.Enum;
using BuildingBlocks.Configurations;
using EasyCaching.Redis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BuildingBlocks.Caching;

/// <summary>
///     Extension methods for configuring custom caching.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Adds custom caching services to the <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The web application builder.</param>
    /// <param name="configuration"></param>
    /// <returns>The web application builder with caching services added.</returns>
    public static IServiceCollection AddCustomCaching(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddValidateOptions<CacheOptions>();
        var cacheOptions = configuration.GetOptions<CacheOptions>(nameof(CacheOptions));
        Guard.Against.Null(
            cacheOptions,
            nameof(cacheOptions));

        services.AddEasyCaching(
            option =>
            {
                if (cacheOptions.RedisCacheOptions is not null)
                {
                    option.UseRedis(
                        config =>
                        {
                            config.DBConfig = new RedisDBOptions
                                              {
                                                  Configuration = cacheOptions.RedisCacheOptions.ConnectionString,
                                              };
                            config.SerializerName = cacheOptions.SerializationType;
                        },
                        nameof(CacheProviderType.Redis));
                }

                option.UseInMemory(
                    config => config.SerializerName = cacheOptions.SerializationType,
                    nameof(CacheProviderType.InMemory));

                switch (cacheOptions.SerializationType)
                {
                    case nameof(CacheSerializationType.Json):
                        option.WithJson(
                            jsonSerializerSettingsConfigure: x => x.TypeNameHandling = TypeNameHandling.None,
                            nameof(CacheSerializationType.Json));
                        break;
                    case nameof(CacheSerializationType.MessagePack):
                        option.WithMessagePack(nameof(CacheSerializationType.MessagePack));
                        break;
                }
            });

        return services;
    }
}
