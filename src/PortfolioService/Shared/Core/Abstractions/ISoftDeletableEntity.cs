namespace PortfolioService.Shared.Core.Abstractions;

public interface ISoftDeletableEntity
{
    DateTime? DeletedOnUtc { get; }
    bool Deleted { get; }
}
