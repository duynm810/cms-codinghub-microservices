using Shared.Dtos.Tag;

namespace Post.Application.Features.V1.Posts.Commons;

public class CreateOrUpdateCommand
{
    public required string Title { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Summary { get; set; }
    
    public string? ThumbnailFileId { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public long CategoryId { get; set; }
    
    public string RawTags { get; set; } = string.Empty;
}