using Shared.Dtos.Category;

namespace Shared.Dtos.Post;

public class PostsByNonStaticPageCategoryDto
{
    public CategoryDto Category { get; set; } = default!;

    public List<PostDto> Posts { get; set; } = default!;
}