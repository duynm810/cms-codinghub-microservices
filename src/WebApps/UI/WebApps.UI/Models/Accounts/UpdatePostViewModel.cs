using Shared.Dtos.Category;
using Shared.Dtos.Post;

namespace WebApps.UI.Models.Accounts;

public class UpdatePostViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
    
    public PostDto Post { get; set; } = default!;

}