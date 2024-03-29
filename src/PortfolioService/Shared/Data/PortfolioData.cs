﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PortfolioService.Shared.Data;

public class PortfolioData
{
    [BsonElement("id")]
    public ObjectId Id { get; set; }

    [BsonElement("currentTotalValue")]
    public float CurrentTotalValue { get; set; }

    [BsonElement("stocks")]
    public ICollection<StockData> Stocks { get; set; }
}

public class StockData
{
    [BsonElement("ticker")]
    public string Ticker { get; set; }

    [BsonElement("baseCurrency")]
    public string BaseCurrency { get; set; }

    [BsonElement("numberOfShares")]
    public int NumberOfShares { get; set; }
}
