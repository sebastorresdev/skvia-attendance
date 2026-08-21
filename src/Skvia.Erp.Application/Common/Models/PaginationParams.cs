namespace Skvia.Erp.Application.Common.Models;

public record PaginationParams
{
    private const int _maxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { init; get; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > _maxPageSize ? _maxPageSize : value <= 0 ? 10 : value;
    }

    public string? SearchTerm { init; get; }
}

