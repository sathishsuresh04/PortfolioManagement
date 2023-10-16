using Mapster;
using PortfolioService.Portfolios.Dtos;
using PortfolioService.Portfolios.Models;

namespace PortfolioService.Portfolios;

public class PortfolioMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Portfolio, PortfolioDto>()
            .ConstructUsing(x => new PortfolioDto(x.Id, x.CurrentTotalValue, x.Stocks.Adapt<ICollection<StockDto>>()));

        config.NewConfig<Stock, StockDto>()
            .ConstructUsing(x => new StockDto(x.Ticker, x.BaseCurrency, x.NumberOfShares));
    }
}
