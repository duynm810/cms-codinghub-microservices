namespace Shared.Dtos.Series;

public class CreateOrUpdateSeriesDto
{
    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? Description { get; set; }

    public string? SeoDescription { get; set; }

    public string? Thumbnail { set; get; }

    public string? Content { get; set; }

    public Guid AuthorUserId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }
}