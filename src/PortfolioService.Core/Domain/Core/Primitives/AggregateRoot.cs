namespace PortfolioService.Core.Domain.Core.Primitives;

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
