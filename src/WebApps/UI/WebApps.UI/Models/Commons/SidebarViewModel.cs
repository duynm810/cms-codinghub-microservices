using Shared.Dtos.Series;

namespace WebApps.UI.Models.Commons;

public class SidebarViewModel
{
    public IEnumerable<SeriesDto> Series { get; set; } = default!;
}