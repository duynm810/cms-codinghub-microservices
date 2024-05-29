using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class PostsByCategoryViewModel
{
    public required CategoryDto Category { get; set; }
    
    public required PagedResponse<PostDto> Posts { get; set; }
    
}