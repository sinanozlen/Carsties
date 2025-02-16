using System;
using MongoDB.Entities;
using SearchService.Model;

namespace SearchService.Services;

public class AucitonSvcHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AucitonSvcHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
        .Sort(x => x.Descending(x => x.UpdateAt))
        .Project(x => x.UpdateAt.ToString())
        .ExecuteAnyAsync();

        return await _httpClient.GetFromJsonAsync<List<Item>>(_config["AuctionServiceUrl"] + "/api/auctions?date=" + lastUpdated);

    }
}
