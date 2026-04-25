namespace Rawnex.Application.Common.Models;

public record PaginatedList<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int TotalPages,
    int TotalCount
)
{
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
