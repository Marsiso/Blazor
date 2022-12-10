﻿namespace Blazor.Shared.Entities.RequestFeatures;

public class RequestParameters
{
    private int _pageSize = 10;
    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > Constants.Constants.MaxPageSize) ? Constants.Constants.MaxPageSize : value;
    }

    public string SearchTerm { get; set; }
    public string OrderBy { get; set; }
}