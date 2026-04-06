namespace Payments.Application.DTOs;

public sealed class PagedResultDto<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public long TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
}
