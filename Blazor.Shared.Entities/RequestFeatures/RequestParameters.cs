namespace Blazor.Shared.Entities.RequestFeatures;

public class RequestParameters
{
    private int _pageSize = 25;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value > 1 ? value : 1;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > Constants.Constants.MaxPageSize) ? Constants.Constants.MaxPageSize : value;
    }

    public string SearchTerm { get; set; }
    public string OrderBy { get; set; }
    public string Fields { get; set; }
}