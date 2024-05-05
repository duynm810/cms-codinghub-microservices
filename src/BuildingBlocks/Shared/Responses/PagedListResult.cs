using Shared.SeedWorks;

namespace Shared.Responses;

public class PagedResponse<T>
{
    public List<T>? Items { get; set; }

    public required MetaData MetaData { get; set; }
}