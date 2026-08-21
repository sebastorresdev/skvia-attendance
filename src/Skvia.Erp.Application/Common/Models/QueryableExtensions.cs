using Microsoft.EntityFrameworkCore;

namespace Skvia.Erp.Application.Common.Models;

public static class QueryableExtensions
{
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var currentPage = pageNumber < 1 ? 1 : pageNumber;
        var validPageSize = pageSize <= 0 ? 10 : pageSize;

        var items = await query
            .Skip((currentPage - 1) * validPageSize)
            .Take(validPageSize)
            .ToListAsync(cancellationToken);

        return PaginatedResponse<T>.Create(items, totalCount, currentPage, validPageSize);
    }
}

