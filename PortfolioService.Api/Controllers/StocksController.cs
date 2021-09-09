using System;
using System.Net.Http;
using CodeTest.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace CodeTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly DataService _dataService;

        public PortfolioController()
        {
            _dataService = new DataService();
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(id)).Result;
            return Ok(portfolio);
        }

        [HttpGet("/value")]
        public IActionResult GetTotalPortfolioValue(string portfolioId, string currency = "USD")
        {
            var portfolio = _dataService.GetPortfolio(ObjectId.Parse(portfolioId)).Result;
            var totalAmount = 0m;
            var stockService = new StockService.StockService();

            foreach (var stock in portfolio.Stocks)
            {
                using (var httpClient = new HttpClient { BaseAddress = new Uri("https://api.currencylayer.com/") })
                {
                    // See https://exchangeratesapi.io/ for details about the api
                    var foo = httpClient.GetAsync(string.Format("latest?base={0}", currency.ToUpper()));
                    var bar = httpClient.GetAsync(string.Format("latest?base={0}", stock.BaseCurrency.ToUpper()));
                    totalAmount += stockService.GetCurrentStockPrice(stock.Ticker).Result.Price * stock.NumberOfShares;
                }
            }

            return Ok(totalAmount);
        }

        [HttpGet("/delete")]
        public IActionResult DeletePortfolio(string portfolioId)
        {
            var dataService = new DataService();
            dataService.DeletePortfolio(ObjectId.Parse(portfolioId));
            return Ok();
        }
    }
}