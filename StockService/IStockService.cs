using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockService
{
    public interface IStockService
    {
        Task<(decimal Price, string NativeCurrency)> GetCurrentStockPrice(string ticker);
    }

    public class StockService : IStockService
    {
        private readonly List<string> _nativeCurrencies = new() { "USD", "SEK", "NOK", "CAD", "EUR" };

        public async Task<(decimal Price, string NativeCurrency)> GetCurrentStockPrice(string ticker)
        {
            var random = new Random();

            return await Task.FromResult((random.Next(20, 800), _nativeCurrencies[random.Next(0, _nativeCurrencies.Count)]));
        }
    }
}