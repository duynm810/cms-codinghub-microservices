namespace Shared.Dtos.PostInSeries;

public class CreatePostInSeriesDto
{
    public Guid SeriesId { get; set; }
    
    public Guid PostId { get; set; }
    
    public int SortOrder { get; set; }
}