using Shared.Dtos.Category;

namespace WebApps.UI.Models;

public class HeaderViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}