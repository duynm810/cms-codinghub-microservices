using Shared.Dtos.Series;

namespace Shared.Dtos.PostInSeries;

public class ManagePostInSeriesDto
{
    public List<SeriesDto> Series { get; set; } = [];

    public List<SeriesDto> CurrentSeries { get; set; } = [];
}