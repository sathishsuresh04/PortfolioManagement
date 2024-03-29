using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using PortfolioService.Portfolios.Models;
using PortfolioService.Shared.Core.Primitives;

namespace PortfolioService.Shared.Data.EntityMappings;

public static class Mapping
{
    public static void Configure()
    {
        BsonClassMap.RegisterClassMap<Entity>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapIdMember(x => x.Id).SetElementName(Constants.ElementsNames.Id);
            });
        BsonClassMap.RegisterClassMap<AggregateRoot>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.SetIsRootClass(true);
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
        BsonClassMap.RegisterClassMap<Portfolio>(
            map =>
            {
                map.AutoMap();
                map.SetIgnoreExtraElements(true);
                map.MapMember(x => x.CurrentTotalValue).SetElementName(Constants.ElementsNames.CurrentTotalValue);
                map.MapMember(x => x.Stocks).SetElementName(Constants.ElementsNames.Stocks);

                // IAuditableEntity
                map.MapMember(x => x.CreatedOnUtc).SetElementName(Constants.ElementsNames.CreatedOnUtc);
                map.MapMember(x => x.ModifiedOnUtc).SetElementName(Constants.ElementsNames.ModifiedOnUtc);

                // ISoftDeletableEntity
                map.MapMember(x => x.DeletedOnUtc).SetElementName(Constants.ElementsNames.DeletedOnUtc);
                map.MapMember(x => x.Deleted).SetElementName(Constants.ElementsNames.Deleted);
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
