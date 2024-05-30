using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models.Posts;

public class PostsByCategoryViewModel
{
    public CategoryDto Category { get; set; } = default!;
    
    public PagedResponse<PostDto> Posts { get; set; } = default!;
    
}