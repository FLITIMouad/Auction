using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services;

public class AuctionSvcHttpClient
{
    private readonly HttpClient _http;
    private readonly IConfiguration _config;

    public AuctionSvcHttpClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDBAsync()
    {
        var lastUpdated = await DB.Find<Item, string>()
        .Sort(x => x.Ascending(i => i.UpdatedAt))
        .Project(x => x.UpdatedAt.ToString())
        .ExecuteFirstAsync();
        var t = _config["AuctionServiceUrl"] + "/api/auctions?date="+lastUpdated;
        return await _http.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] + "/api/auctions?date="+lastUpdated);
    }
}
