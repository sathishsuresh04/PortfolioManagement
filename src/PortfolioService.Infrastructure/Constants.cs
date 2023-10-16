namespace PortfolioService.Infrastructure;

internal static class Constants
{
    internal static class AppSettings
    {
        internal const string MongoDbOptions = "MongoDbOptions";
    }

    internal static class ElementsNames
    {
        internal const string Id = "id";
        internal const string CurrentTotalValue = "currentTotalValue";
        internal const string Stocks = "stocks";
        internal const string CreatedOnUtc = "createdOnUtc";
        internal const string ModifiedOnUtc = "modifiedOnUtc";
        internal const string DeletedOnUtc = "deletedOnUtc";
        internal const string Deleted = "deleted";

        internal const string Ticker = "ticker";
        internal const string BaseCurrency = "baseCurrency";
        internal const string NumberOfShares = "numberOfShares";
    }
}
