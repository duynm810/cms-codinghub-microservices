
namespace Shared.Dtos.PostInSeries;

public class PostInSeriesDto
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Slug { get; set; }
}