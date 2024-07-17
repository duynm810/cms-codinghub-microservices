using Shared.Dtos.Series;

namespace WebApps.UI.Models.PostInSeries;

public class ManageSeriesViewModel
{
    public List<SeriesDto> Series { get; set; } = [];

    public SeriesDto? CurrentSeries { get; set; }
}