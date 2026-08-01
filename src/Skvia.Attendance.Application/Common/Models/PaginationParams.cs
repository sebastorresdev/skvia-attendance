namespace Skvia.Attendance.Application.Common.Models;

public record PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { init; get; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value <= 0 ? 10 : value;
    }

    public string? SearchTerm { init; get; }
}
