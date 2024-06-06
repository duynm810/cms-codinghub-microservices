using Shared.Dtos.Category;
using Shared.Dtos.Tag;

namespace WebApps.UI.Models.Commons;

public class FooterViewModel : BaseViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;

    public IEnumerable<TagDto> Tags { get; set; } = default!;
}