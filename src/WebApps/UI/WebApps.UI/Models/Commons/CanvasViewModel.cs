using Shared.Dtos.Series;

namespace WebApps.UI.Models.Commons;

public class CanvasViewModel : BaseViewModel
{
    public IEnumerable<SeriesDto> Series { get; set; } = default!;
}