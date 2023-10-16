namespace PortfolioService.Core.Domain.Core.Abstractions;

public interface ISoftDeletableEntity
{
    DateTime? DeletedOnUtc { get; }
    bool Deleted { get; }
}
