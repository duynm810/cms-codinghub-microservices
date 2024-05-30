namespace Shared.Dtos.Post;

public class PostDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public string? Tags { get; set; }

    public int ViewCount { get; set; }

    public bool IsPaid { get; set; }

    public DateTimeOffset PublishedDate { get; set; }

    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public string? CategoryIcon { get; set; }

    public string? CategoryColor { get; set; }

    public Guid AuthorUserId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}