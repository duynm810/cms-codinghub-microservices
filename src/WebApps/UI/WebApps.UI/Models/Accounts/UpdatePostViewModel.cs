using Shared.Dtos.Category;
using Shared.Dtos.Post;

namespace WebApps.UI.Models.Accounts;

public class UpdatePostViewModel
{
    public List<CategoryDto> Categories { get; set; } = [];
    
    public PostDto Post { get; set; } = default!;

}