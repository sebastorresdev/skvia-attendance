namespace Skvia.Attendance.Application.Common.Models;

public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage)
{
    public static PaginatedResponse<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var currentPage = pageNumber < 1 ? 1 : pageNumber;

        return new PaginatedResponse<T>(
            items,
            currentPage,
            pageSize,
            totalCount,
            totalPages,
            HasPreviousPage: currentPage > 1,
            HasNextPage: currentPage < totalPages);
    }
}
