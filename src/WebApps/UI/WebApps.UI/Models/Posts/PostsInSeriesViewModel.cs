using Shared.Dtos.PostInSeries;
using Shared.Dtos.Series;
using Shared.Responses;
using WebApps.UI.Models.Commons;

namespace WebApps.UI.Models.Posts;

public class PostsInSeriesViewModel
{
    public SeriesDto Series { get; set; } = default!;

    public PagedResponse<PostInSeriesDto> PostInSeries { set; get; } = default!;
}