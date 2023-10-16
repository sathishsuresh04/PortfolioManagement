using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using PortfolioService.Core.Domain.Core.Abstractions;
using PortfolioService.Core.Domain.Core.Primitives;
using PortfolioService.Core.Domain.Entities;

namespace PortfolioService.Infrastructure.Persistence.EntityMappings;

public static class Mapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<AggregateRoot>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id).SetElementName(Constants.ElementsNames.Id);
                map.SetIsRootClass(true);
            });
        BsonClassMap.RegisterClassMap<Entity>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id).SetElementName(Constants.ElementsNames.Id);
            });

        BsonClassMap.RegisterClassMap<IAuditableEntity>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.CreatedOnUtc).SetElementName(Constants.ElementsNames.CreatedOnUtc);
                map.MapMember(x => x.ModifiedOnUtc).SetElementName(Constants.ElementsNames.ModifiedOnUtc);
            });
        BsonClassMap.RegisterClassMap<ISoftDeletableEntity>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.DeletedOnUtc).SetElementName(Constants.ElementsNames.DeletedOnUtc);
                map.MapMember(x => x.Deleted).SetElementName(Constants.ElementsNames.Deleted);
            });

        BsonClassMap.RegisterClassMap<Portfolio>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.CurrentTotalValue).SetElementName(Constants.ElementsNames.CurrentTotalValue);
                map.MapMember(x => x.Stocks).SetElementName(Constants.ElementsNames.Stocks);
            });
        BsonClassMap.RegisterClassMap<Stock>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.Ticker).SetElementName(Constants.ElementsNames.Ticker);
                map.MapMember(x => x.BaseCurrency).SetElementName(Constants.ElementsNames.BaseCurrency);
                map.MapMember(x => x.NumberOfShares).SetElementName(Constants.ElementsNames.NumberOfShares);
            });
        // Conventions
        var pack = new ConventionPack
                   {
                       new IgnoreExtraElementsConvention(true),
                       new IgnoreIfDefaultConvention(true),
                       new CamelCaseElementNameConvention(),
                   };
        var baseDocumentEntityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(
                domainAssembly => domainAssembly.GetTypes(),
                (domainAssembly, assemblyType) => new {domainAssembly, assemblyType,})
            .Where(t => t.assemblyType.IsSubclassOf(typeof(Entity)))
            .Select(t => t.assemblyType)
            .ToArray();

        foreach (var baseDocumentEntityType in baseDocumentEntityTypes)
        {
            BsonSerializer.RegisterDiscriminatorConvention(
                baseDocumentEntityType,
                NullDiscriminatorConvention.Instance);
        }

        ConventionRegistry.Register("Portfolio Service Conventions", pack, _ => true);
    }
}
