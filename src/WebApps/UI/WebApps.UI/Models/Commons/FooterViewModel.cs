using Shared.Dtos.Category;
using Shared.Dtos.Tag;

namespace WebApps.UI.Models.Commons;

public class FooterViewModel
{
    public List<CategoryDto> Categories { get; set; } = [];

    public List<TagDto> Tags { get; set; } = [];
}