using System.Text.Json;
using Ardalis.GuardClauses;
using BuildingBlocks.Abstractions.CQRS;
using BuildingBlocks.Core.Exceptions;
using BuildingBlocks.Validator;
using FluentValidation;
using PortfolioService.Portfolios.Data.Abstractions;
using PortfolioService.Portfolios.Exceptions;
using PortfolioService.Portfolios.Models.ValueObjects;
using PortfolioService.Shared.Data;
using PortfolioService.Shared.Dtos;

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

    public GetTotalPortfolioValueHandler(IPortfolioRepository portfolioRepository)
    {
        _portfolioRepository = portfolioRepository;
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
        var stockService = new StockService();
        var apiAccessKey = "edcbcd5977de259ca7fb25077ca8a0f6";
        using (var httpClient = new HttpClient {BaseAddress = new Uri("http://api.currencylayer.com/"),})
        {
            // See https://currencylayer.com/documentation for details about the api
            var foo = await httpClient.GetAsync($"live?access_key={apiAccessKey}", cancellationToken);
            var data = await JsonSerializer.DeserializeAsync<Quote>(
                           await foo.Content.ReadAsStreamAsync(cancellationToken),
                           cancellationToken: cancellationToken);

            foreach (var stock in portfolio.Stocks)
            {
                if (stock.BaseCurrency == request.Currency)
                {
                    totalAmount += (await stockService.GetCurrentStockPrice(stock.Ticker)).Price *
                                   stock.NumberOfShares;
                }
                else
                {
                    if (request.Currency == "USD")
                    {
                        var stockPrice = (await stockService.GetCurrentStockPrice(stock.Ticker)).Price;
                        if (data == null) continue;
                        var rateUsd = data.quotes["USD" + stock.BaseCurrency];
                        totalAmount += stockPrice / rateUsd * stock.NumberOfShares;
                    }
                    else
                    {
                        var stockPrice = (await stockService.GetCurrentStockPrice(stock.Ticker)).Price;
                        if (data == null) continue;
                        var rateUsd = data.quotes["USD" + stock.BaseCurrency];
                        var amount = stockPrice / rateUsd * stock.NumberOfShares;
                        var targetRateUsd = data.quotes["USD" + request.Currency];
                        totalAmount += amount * targetRateUsd;
                    }
                }
            }
        }


        return new GetTotalPortfolioValueResponse(totalAmount);
    }
}
