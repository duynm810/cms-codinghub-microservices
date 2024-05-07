namespace Shared.Dtos.Post;

public class CreateOrUpdatePostDto
{
    public required string Name { get; set; }

    public required string Slug { get; set; }

    public string? Content { get; set; }

    public string? Description { get; set; }

    public string? Thumbnail { get; set; }

    public string? SeoDescription { get; set; }

    public string? Source { get; set; }

    public string? Tags { get; set; }

    public double RoyaltyAmount { get; set; }

    public long CategoryId { get; set; }
}