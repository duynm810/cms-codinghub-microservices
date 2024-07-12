namespace Shared.Requests.PostInSeries;

public class DeletePostInSeriesRequest
{
    public Guid SeriesId { get; set; }

    public Guid PostId { get; set; }
}