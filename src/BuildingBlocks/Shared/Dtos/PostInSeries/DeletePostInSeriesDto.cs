namespace Shared.Dtos.PostInSeries;

public class DeletePostInSeriesDto
{
    public Guid SeriesId { get; set; }
    
    public Guid PostId { get; set; }
}