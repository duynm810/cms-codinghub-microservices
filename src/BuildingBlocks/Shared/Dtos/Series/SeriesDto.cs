namespace Shared.Dtos.Series;

public class SeriesDto
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Slug { get; set; }

    public string? Description { get; set; }

    public string? SeoDescription { get; set; }

    public string? Content { get; set; }

    public Guid AuthorUserId { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}