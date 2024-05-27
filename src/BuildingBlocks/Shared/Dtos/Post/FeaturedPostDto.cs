using Shared.Enums;

namespace Shared.Dtos.Post;

public class FeaturedPostDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public string? Tags { get; set; }

    public int ViewCount { get; set; }

    public bool IsPaid { get; set; }

    public double RoyaltyAmount { get; set; }

    public PostStatusEnum Status { get; set; }

    public long CategoryId { get; set; }
    
    public string? CategoryName { get; set; }

    public Guid AuthorUserId { get; set; }

    public DateTimeOffset? PaidDate { get; set; }
    
    public DateTimeOffset CreatedDate { get; set; }
}