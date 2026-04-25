using Microsoft.EntityFrameworkCore;

namespace Rawnex.Application.Common.Models;

public static class PaginatedListExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var count = await source.CountAsync(ct);
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        return new PaginatedList<T>(items, pageNumber, totalPages, count);
    }
}
