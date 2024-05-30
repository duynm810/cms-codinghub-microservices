using Shared.Dtos.Category;

namespace WebApps.UI.Models.Commons;

public class FooterViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}