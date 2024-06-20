using Shared.Dtos.Category;
using Shared.Dtos.Post.Queries;

namespace WebApps.UI.Models.Accounts;

public class UpdatePostViewModel
{
    public PostDto Post { get; set; } = default!;
    
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}