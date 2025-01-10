using Microsoft.EntityFrameworkCore;

namespace FUC.Common.Shared;

public class PaginatedList<T>
{
    public IEnumerable<T> Items { get; }

    public long TotalNumberOfItems { get; }

    public int CurrentPage { get; }

    public PaginatedList(IEnumerable<T> items, long totalNumberOfItems, int currentPage)
    {
        Items = items;
        TotalNumberOfItems = totalNumberOfItems;
        CurrentPage = currentPage;
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source,
        int page,
        int numberOfItems,
        CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfItems);

        var totalNumberOfItems = await source.LongCountAsync(cancellationToken);

        if (totalNumberOfItems == 0)
        {
            return new PaginatedList<T>([], totalNumberOfItems, default);
        }

        var totalNumberOfPages = (int)Math.Ceiling(totalNumberOfItems / (decimal)numberOfItems);

        if (page <= 0)
        {
            page = 1;
        }

        if (page > totalNumberOfPages)
        {
            page = totalNumberOfPages;
        }

        var skip = (page - 1) * numberOfItems;

        var items = await source
            .Skip(skip)
            .Take(numberOfItems)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, totalNumberOfItems, page);
    }
}
