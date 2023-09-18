﻿namespace SearchService.RequestHelper;

public class SearchParams
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 4;
    public string Seller { get; set; }
    public string Winner { get; set; }
    public string OrderBY { get; set; }
    public string FiltterBy { get; set; }
}
