using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel
{
    public IEnumerable<PostsByNonStaticPageCategoryDto> PostsWithCategory { get; set; } = default!;
}