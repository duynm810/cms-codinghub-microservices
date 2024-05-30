using Shared.Dtos.Category;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}