using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Models;

public class PostsByCategoryViewModel
{
    public CategoryDto Category { get; set; }
    
    public PagedResponse<PostByCategoryDto> Posts { get; set; }
    
}