using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Validator;
using FluentValidation;
using Microsoft.Extensions.Options;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Models.ValueObjects;
using PortfolioService.Shared.Abstractions;
using PortfolioService.Shared.Options;

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
    private readonly IExchangeRateApiClient _exchangeRateApiClient;
    private readonly ExchangeRateApiOptions _exchangeRateApiOptions;
    private readonly IPortfolioRepository _portfolioRepository;
    private readonly IStockService _stockService;

    public GetTotalPortfolioValueHandler(
        IPortfolioRepository portfolioRepository,
        IExchangeRateApiClient exchangeRateApiClient,
        IStockService stockService,
        IOptions<ExchangeRateApiOptions> options
    )
    {
        _portfolioRepository = portfolioRepository;
        _exchangeRateApiClient = exchangeRateApiClient;
        _stockService = stockService;
        _exchangeRateApiOptions = options.Value;
    }

    public async Task<GetTotalPortfolioValueResponse> Handle(
        GetTotalPortfolioValue request,
        CancellationToken cancellationToken
    )
    {
        Guard.Against.Null(request);
        var portfolio = await _portfolioRepository.GetByIdAsync(PortfolioId.Of(request.Id));
        Guard.Against.NotFound(portfolio, new PortfolioNotFoundException(request.Id));

        var totalAmount = 0m;

        var data = await _exchangeRateApiClient.GetExchangeRateAsync(_exchangeRateApiOptions.Token);
        foreach (var stock in portfolio.Stocks)
        {
            if (stock.BaseCurrency == request.Currency)
            {
                totalAmount += (await _stockService.GetCurrentStockPrice(stock.Ticker)).Price *
                               stock.NumberOfShares;
            }
            else
            {
                if (request.Currency == "USD")
                {
                    var stockPrice = (await _stockService.GetCurrentStockPrice(stock.Ticker)).Price;
                    if (data == null) continue;
                    var rateUsd = data.quotes["USD" + stock.BaseCurrency];
                    totalAmount += stockPrice / rateUsd * stock.NumberOfShares;
                }
                else
                {
                    var stockPrice = (await _stockService.GetCurrentStockPrice(stock.Ticker)).Price;
                    if (data == null) continue;
                    var rateUsd = data.quotes["USD" + stock.BaseCurrency];
                    var amount = stockPrice / rateUsd * stock.NumberOfShares;
                    var targetRateUsd = data.quotes["USD" + request.Currency];
                    totalAmount += amount * targetRateUsd;
                }
            }
        }

        return new GetTotalPortfolioValueResponse(totalAmount);
    }
}
