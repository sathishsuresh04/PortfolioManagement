namespace PortfolioService.Portfolios.Dtos;

public record PortfolioDto(string Id, float CurrentTotalValue,DateTime CreatedOn,DateTime? ModifiedOn, DateTime? DeletedOn,bool Deleted, ICollection<StockDto> Stocks);
