using Shared.Dtos.Category;

namespace Shared.Dtos.Post.Queries;

public class PostsByNonStaticPageCategoryDto
{
    public CategoryDto Category { get; set; } = default!;

    public List<PostDto> Posts { get; set; } = default!;
}