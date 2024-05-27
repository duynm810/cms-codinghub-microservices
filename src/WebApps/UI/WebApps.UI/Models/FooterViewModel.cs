using Shared.Dtos.Category;

namespace WebApps.UI.Models;

public class FooterViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
}