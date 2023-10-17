using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Configurations;

/// <summary>
///     Static helper class for <see cref="IConfiguration" />.
/// </summary>
public static class ConfigurationExtensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section)
    where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);
        return model;
    }

    /// <summary>
    ///     Registers an options instance and configures it using data annotations.
    ///     This instance will be registered as a singleton.
    ///     This method won't throw exceptions immediately if configuration validation fails,
    ///     but will defer the error until the options are accessed.
    /// </summary>
    /// <typeparam name="TModel">The type of options to register.</typeparam>
    /// <param name="service">The IServiceCollection to add the services to.</param>
    public static void AddValidateOptions<TModel>(this IServiceCollection service)
    where TModel : class, new()
    {
        service.AddOptions<TModel>()
            .BindConfiguration(typeof(TModel).Name)
            .ValidateDataAnnotations();

        service.AddSingleton(x => x.GetRequiredService<IOptions<TModel>>().Value);
    }

    /// <summary>
    ///     Binds the app settings to an instance of type T and validates it using any applied annotations.
    ///     If validation is successful, the method configures TOptions and adds the instance to the services as a singleton.
    ///     This method implements a "fail-fast" approach and will throw an exception immediately if configuration validation
    ///     fails.
    /// </summary>
    /// <typeparam name="T">The type of object to bind the settings to.</typeparam>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="sectionKey">The name of the configuration section to bind.</param>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <returns>The instance of T with bound and validated settings.</returns>
    public static T AddValidateOptions<T>(
        this IServiceCollection services,
        string sectionKey,
        IConfiguration configuration
    )
    where T : class, new()
    {
        var section = GetSection(configuration, sectionKey);
        var instance = new T();
        section.Bind(instance);
        System.ComponentModel.DataAnnotations.Validator.ValidateObject(instance, new ValidationContext(instance), true);

        services.Configure<T>(options => section.Bind(options));
        services.AddSingleton(instance);

        return instance;
    }

    /// <summary>
    ///     Retrieves a configuration section by key. Throws an ArgumentNullException if the section does not exist.
    /// </summary>
    /// <param name="configuration">The IConfiguration instance.</param>
    /// <param name="sectionKey">The key of the configuration section.</param>
    /// <returns>The IConfiguration section.</returns>
    private static IConfiguration GetSection(
        IConfiguration configuration,
        string sectionKey
    )
    {
        var section = configuration.GetSection(sectionKey);
        if (string.IsNullOrWhiteSpace(section.Key))
            throw new ArgumentNullException($"Key {sectionKey} is missing in the configuration.");
        return section;
    }
}
