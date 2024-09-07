using System.Reflection;
using BuildingBlocks.Abstractions.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
///     Default implementation of the <see cref="ISerializer" /> interface using JSON serialization.
/// </summary>
public class DefaultSerializer : ISerializer
{
    /// <inheritdoc />
    public string ContentType => "application/json";

    /// <inheritdoc />
    public string Serialize(object obj, bool camelCase = true, bool indented = true)
    {
        return JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));
    }

    /// <inheritdoc />
    public T? Deserialize<T>(string payload, bool camelCase = true)
    {
        return JsonConvert.DeserializeObject<T>(payload, CreateSerializerSettings(camelCase));
    }

    /// <inheritdoc />
    public object? Deserialize(string payload, Type type, bool camelCase = true)
    {
        return JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings(camelCase));
    }

    /// <summary>
    ///     Creates the <see cref="JsonSerializerSettings" /> used for serialization and deserialization.
    /// </summary>
    /// <param name="camelCase">Indicates whether to use camel case for property names.</param>
    /// <param name="indented">Indicates whether to indent the output.</param>
    /// <returns>The serializer settings.</returns>
    protected static JsonSerializerSettings CreateSerializerSettings(bool camelCase = true, bool indented = false)
    {
        var settings = new JsonSerializerSettings {ContractResolver = new ContractResolverWithPrivate(),};

        if (indented) settings.Formatting = Formatting.Indented;

        // For handling private constructors
        settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        // Custom converter for DateOnly type
        settings.Converters.Add(new DateOnlyConverter());

        return settings;
    }

    /// <summary>
    ///     Custom contract resolver that allows serialization of private setters.
    /// </summary>
    private sealed class ContractResolverWithPrivate : CamelCasePropertyNamesContractResolver
    {
        // Reference: http://danielwertheim.se/json-net-private-setters/
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    var hasPrivateSetter = property.GetSetMethod(true) != null;
                    prop.Writable = hasPrivateSetter;
                }
            }

            return prop;
        }
    }
}
