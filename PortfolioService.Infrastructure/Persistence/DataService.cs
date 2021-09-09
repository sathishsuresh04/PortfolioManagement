using System.Threading.Tasks;
using CodeTest.Infrastructure.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CodeTest.Infrastructure.Persistence
{
    public class DataService
    {
        private readonly IMongoCollection<PortfolioData> _portfolioCollection;

        public DataService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            _portfolioCollection = client.GetDatabase("portfolioServiceDb").GetCollection<PortfolioData>("Portfolios");
        }

        public async Task<PortfolioData> GetPortfolio(ObjectId id)
        {
            var idFilter = Builders<PortfolioData>.Filter.Eq(portfolio => portfolio.Id, id);

            return await _portfolioCollection.Find(idFilter).FirstOrDefaultAsync();
        }

        public async Task DeletePortfolio(ObjectId id)
        {
            await _portfolioCollection.DeleteOneAsync(Builders<PortfolioData>.Filter.Eq(portfolio => portfolio.Id, id));
        }
    }
}