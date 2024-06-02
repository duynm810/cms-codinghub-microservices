using Shared.Dtos.Category;

namespace WebApps.UI.Models.Commons;

public class HeaderViewModel : BaseViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;
}