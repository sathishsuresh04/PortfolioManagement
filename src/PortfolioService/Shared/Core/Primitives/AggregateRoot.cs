namespace PortfolioService.Shared.Core.Primitives;

public class AggregateRoot : Entity
{
    protected AggregateRoot(string id)
        : base(id)
    {
    }

    protected AggregateRoot()
    {
    }
}
