namespace Shared.Models;

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
        PageNumber = 0;
        PageSize = 10;
    }
}