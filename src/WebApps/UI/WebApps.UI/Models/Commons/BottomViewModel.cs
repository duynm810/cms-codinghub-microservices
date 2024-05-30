using Shared.Dtos.Category;
using Shared.Dtos.Series;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel
{
    public IEnumerable<CategoryDto> Categories { get; set; } = default!;

    public IEnumerable<SeriesDto> Series { get; set; } = default!;
}