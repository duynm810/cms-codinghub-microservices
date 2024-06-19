using Shared.Dtos.Category;

namespace Shared.Dtos.Post.Queries;

public class PostsByNonStaticPageCategoryDto
{
    public CategoryDto Category { get; set; }

    public List<PostDto>? Posts { get; set; }
}