using MongoDB.Bson;

namespace PortfolioService.Shared.Core.Primitives;

public class AggregateRoot : Entity
{
    protected AggregateRoot(ObjectId id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
