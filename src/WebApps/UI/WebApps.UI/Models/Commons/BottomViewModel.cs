using Shared.Dtos.Post;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel : BaseViewModel
{
    public IEnumerable<PostsByNonStaticPageCategoryDto> PostsWithCategory { get; set; } = default!;
}