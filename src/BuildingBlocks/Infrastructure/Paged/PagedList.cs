using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Shared.SeedWorks;

namespace Infrastructure.Paged;

public class PagedList<T> : List<T>
{
    private PagedList(IEnumerable<T> items, long totalItems, int pageIndex, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = pageIndex,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };

        AddRange(items);
    }

    private MetaData MetaData { get; }

    public MetaData GetMetaData()
    {
        return MetaData;
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize,
        Expression<Func<T, object>> orderBy)
    {
        var count = await source.CountAsync();
        var items = await source
            .OrderBy(orderBy)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize, Func<T, object> orderBy)
    {
        var sourceList = source.ToList();

        var ordered = sourceList.OrderBy(orderBy).ToList();

        var count = ordered.Count;
        var items = ordered
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}