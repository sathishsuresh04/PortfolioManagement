namespace PortfolioService.Portfolios.Dtos;

public record PortfolioDto(string Id, float CurrentTotalValue, ICollection<StockDto> Stocks);
