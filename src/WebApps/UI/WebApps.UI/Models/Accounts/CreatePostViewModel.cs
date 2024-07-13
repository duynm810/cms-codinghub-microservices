using Shared.Dtos.Category;
using Shared.Requests.Post.Commands;

namespace WebApps.UI.Models.Accounts;

public class CreatePostViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;

    public CreatePostRequest Post { get; set; } = default!;
}