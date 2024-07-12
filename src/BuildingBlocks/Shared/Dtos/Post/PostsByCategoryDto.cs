using Shared.Dtos.Category;
using Shared.Responses;

namespace Shared.Dtos.Post;

public class PostsByCategoryDto
{
    public CategoryDto Category { get; set; } = default!;
    
    public PagedResponse<PostDto> Posts { get; set; } = default!;
}