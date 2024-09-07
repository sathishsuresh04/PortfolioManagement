using System.Globalization;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Serialization;

/// <summary>
///     Custom JSON converter for serializing and deserializing <see cref="DateOnly" /> objects.
/// </summary>
public class DateOnlyConverter : JsonConverter<DateOnly>
{
    private const string Format = "yyyy-MM-dd";

    /// <inheritdoc />
    public override DateOnly ReadJson(
        JsonReader reader,
        Type objectType,
        DateOnly existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        return DateOnly.ParseExact((string)reader.Value!, Format, CultureInfo.InvariantCulture);
    }

    /// <inheritdoc />
    public override void WriteJson(JsonWriter writer, DateOnly value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString(Format, CultureInfo.InvariantCulture));
    }
}
