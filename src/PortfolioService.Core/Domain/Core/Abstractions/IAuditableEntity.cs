namespace PortfolioService.Core.Domain.Core.Abstractions;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; }
    DateTime? ModifiedOnUtc { get; }
}
