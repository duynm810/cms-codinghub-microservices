namespace Shared.Dtos.PostInSeries;

public class PostInSeriesDto
{
    public Guid Id { get; set; }

    public string? Title { get; set; }

    public string? Slug { get; set; }

    public int ViewCount { get; set; }

    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public string? CategoryColor { get; set; }
}