using Shared.Dtos.Post;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel
{
    public IEnumerable<PostsByNonStaticPageCategoryDto> PostsWithCategory { get; set; } = default!;
}