namespace Cases.Application.Common.Models;

public record PaginationQuery(int PageNumber = 1, int PageSize = 10)
{
    public int Skip => (PageNumber - 1) * PageSize;
}
