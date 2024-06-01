using Shared.Dtos.Post;

namespace WebApps.UI.Models.Posts;

public class PostsByNonStaticPageCategoryViewModel
{
    public long CategoryId { get; set; }

    public string? CategoryName { get; set; }
    
    public string? CategorySlug { get; set; }

    public List<PostDto>? Posts { get; set; }
}