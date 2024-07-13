namespace Shared.Models.Common;

public class Pagination
{
    /// <summary>
    /// Page
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Size
    /// </summary>
    public int PageSize { get; set; }

    protected Pagination()
    {
        PageNumber = 1;
        PageSize = 10;
    }
}