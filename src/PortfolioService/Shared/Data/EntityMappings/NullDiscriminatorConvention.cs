using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace PortfolioService.Shared.Data.EntityMappings;

public class NullDiscriminatorConvention : IDiscriminatorConvention
{
    public static NullDiscriminatorConvention Instance { get; } = new();

    public Type GetActualType(IBsonReader bsonReader, Type nominalType)
    {
        return nominalType;
    }

    public BsonValue GetDiscriminator(Type nominalType, Type actualType)
    {
        return null;
    }

    public string ElementName { get; } = null;
}
