using Shared.Dtos.Series;
using Shared.Responses;

namespace Shared.Dtos.Post.Queries;

public class PostsBySeriesDto
{
    public SeriesDto Series { get; set; } = default!;

    public PagedResponse<PostDto> Posts { set; get; } = default!;
}