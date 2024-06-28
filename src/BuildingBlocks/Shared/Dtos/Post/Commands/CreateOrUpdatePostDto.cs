namespace Shared.Dtos.Post.Commands;

public class CreateOrUpdatePostDto
{
    public required string Title { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Summary { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }
    
    public long CategoryId { get; set; }

    public string RawTags { get; set; } = string.Empty;
}