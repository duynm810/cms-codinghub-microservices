using Shared.Dtos.Category;
using Shared.Dtos.Post;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostsByCategoryViewModel : BaseViewModel
{
    public CategoryDto Category { get; set; } = default!;
    
    public PagedResponse<PostDto> Posts { get; set; } = default!;
    
}