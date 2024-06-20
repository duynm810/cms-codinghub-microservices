
using Shared.Dtos.Category;

namespace WebApps.UI.Models.Accounts;

public class CreatePostViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}