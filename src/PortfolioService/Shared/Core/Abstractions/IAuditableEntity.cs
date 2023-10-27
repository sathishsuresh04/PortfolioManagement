namespace PortfolioService.Shared.Core.Abstractions;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; }
    DateTime? ModifiedOnUtc { get; }
}
