using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelper;

namespace SearchService.Controllers;
[Route("api/search")]
[ApiController]
public class SearchController : ControllerBase
{


    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();

        query = searchParams.OrderBY switch
        {
            "make" => query.Sort(x => x.Ascending(i => i.Make)),
            "new" => query.Sort(x => x.Ascending(i => i.CreatedAt)),
            _ => query.Sort(x => x.Ascending(i => i.AuctionEnd))
        };
        query = searchParams.FiltterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };

        if (!string.IsNullOrWhiteSpace(searchParams.Seller))
            query.Match(x => x.Seller == searchParams.Seller);
        if (!string.IsNullOrWhiteSpace(searchParams.Winner))
            query.Match(x => x.Winner == searchParams.Winner);


        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result=await query.ExecuteAsync();
        return Ok(new {
            results=result.Results, 
            pageCount=result.PageCount, 
            totalCount=result.TotalCount
        });
    }
}
