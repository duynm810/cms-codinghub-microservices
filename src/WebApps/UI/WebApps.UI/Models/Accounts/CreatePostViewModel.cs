
using Shared.Dtos.Category;
using Shared.Dtos.Post.Commands;

namespace WebApps.UI.Models.Accounts;

public class CreatePostViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;

    public CreatePostDto Post { get; set; } = default!;
}