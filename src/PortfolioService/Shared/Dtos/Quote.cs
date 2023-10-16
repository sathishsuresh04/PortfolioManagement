namespace PortfolioService.Shared.Dtos;

public class Quote
{
    public bool success { get; set; }
    public string terms { get; set; }
    public string privacy { get; set; }
    public int timestamp { get; set; }
    public string source { get; set; }
    public Dictionary<string, decimal> quotes { get; set; }
}
