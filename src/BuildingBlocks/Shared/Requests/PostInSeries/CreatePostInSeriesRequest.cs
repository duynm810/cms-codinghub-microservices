namespace Shared.Requests.PostInSeries;

public class CreatePostInSeriesRequest
{
    public Guid SeriesId { get; set; }

    public List<Guid> PostIds { get; set; } = [];
}