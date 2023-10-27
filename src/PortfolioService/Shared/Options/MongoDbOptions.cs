using System.ComponentModel.DataAnnotations;

namespace PortfolioService.Shared.Options;

public class MongoDbOptions
{
    [Required]
    public string DatabaseName { get; init; } = null!;

    [Required]
    public string ConnectionString { get; init; } = null!;
}
