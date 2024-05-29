using Shared.Enums;

namespace Shared.Dtos.Post;

public class FeaturedPostDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }
    
    public int ViewCount { get; set; }

    public long CategoryId { get; set; }
    
    public string? CategoryName { get; set; }
    
    public string? CategorySlug { get; set; }

    public string? CategoryIcon { get; set; }
    
    public string? CategoryColor { get; set; }

    public Guid AuthorUserId { get; set; }

    public DateTimeOffset CreatedDate { get; set; }
}