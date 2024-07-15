using Shared.Dtos.Post;

namespace WebApps.UI.Models.Commons;

public class BottomViewModel
{
    public List<PostsByNonStaticPageCategoryDto> PostsWithCategory { get; set; } = [];
}