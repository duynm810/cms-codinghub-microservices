namespace Shared.Requests.PostInSeries;

public class CreatePostInSeriesRequest
{
    public Guid SeriesId { get; set; }

    public Guid PostId { get; set; }

    public int SortOrder { get; set; }
}