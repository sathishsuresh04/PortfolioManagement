using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Validator;
using FluentValidation;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Models.ValueObjects;
using PortfolioService.Shared.Abstractions;
using PortfolioService.Shared.Dtos;
using PortfolioService.Shared.Infrastructure;

namespace PortfolioService.Portfolios.Features.GetTotalPortfolioValue;

public record GetTotalPortfolioValue(string Id, string Currency = "USD") : IQuery<GetTotalPortfolioValueResponse>
{
    internal sealed class GetTotalPortfolioValueValidator : BaseValidator<GetTotalPortfolioValue>
    {
        public GetTotalPortfolioValueValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required!");
        }
    }
}

public record GetTotalPortfolioValueResponse(decimal TotalValue);

internal sealed class
    GetTotalPortfolioValueHandler : IQueryHandler<GetTotalPortfolioValue, GetTotalPortfolioValueResponse>
{

    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockService _stockService;
    private readonly ExchangeRateService _exchangeRateService;

    public GetTotalPortfolioValueHandler(
        IPortfolioRepository portfolioRepository,
        IStockService stockService,
        ExchangeRateService exchangeRateService
    )
    {
        _portfolioRepository = portfolioRepository;
        _stockService = stockService;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<GetTotalPortfolioValueResponse> Handle(
        GetTotalPortfolioValue request,
        CancellationToken cancellationToken)
    {
        Guard.Against.Null(request);
        var portfolio = await _portfolioRepository.GetByIdAsync(PortfolioId.Of(request.Id));
        Guard.Against.NotFound(portfolio, new PortfolioNotFoundException(request.Id));

        var data = await _exchangeRateService.GetExchangeRateAsync(cancellationToken);
        var totalAmount = 0m;

        foreach (var stock in portfolio.Stocks)
        {
            var stockPriceInfo = await _stockService.GetCurrentStockPrice(stock.Ticker);
            var convertedPrice = CalculateConvertedStockPrice(stockPriceInfo.Price, stock.BaseCurrency, request.Currency, data);
            totalAmount += convertedPrice * stock.NumberOfShares;
        }

        return new GetTotalPortfolioValueResponse(totalAmount);
    }

    private static decimal CalculateConvertedStockPrice(decimal stockPrice, string baseCurrency, string targetCurrency, Quote data)
    {
        if (baseCurrency == targetCurrency)
        {
            return stockPrice;
        }

        if (data == null)
        {
            throw new Exception("Exchange data cannot be null when converting currencies.");
        }

        var rateToUsd = baseCurrency == "USD" ? 1m : // 1 USD = 1 USD  treat the condition where baseCurrency is "USD" as a special case
                            data.quotes["USD" + baseCurrency];

        if (targetCurrency  == "USD")
        {
            return stockPrice / rateToUsd;
        }

        var targetRate = data.quotes["USD" + targetCurrency];
        return (stockPrice / rateToUsd) * targetRate;
    }
}
