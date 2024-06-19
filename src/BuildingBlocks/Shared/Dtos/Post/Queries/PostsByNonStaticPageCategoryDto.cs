namespace Shared.Dtos.Post.Queries;

public class PostsByNonStaticPageCategoryDto
{
    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public List<PostDto>? Posts { get; set; }
}