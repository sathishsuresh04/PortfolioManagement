namespace PortfolioService.Shared.Options;

public record ExchangeRateApiOptions
{
    public string Token { get; init; } = default!;
    public string BaseApiAddress { get; init; } = default!;
}
