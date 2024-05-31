
namespace Shared.Dtos.PostInSeries;

public class PostInSeriesDto
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Slug { get; set; }
}