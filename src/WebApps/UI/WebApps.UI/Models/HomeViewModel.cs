using Shared.Dtos.Post;

namespace WebApps.UI.Models;

public class HomeViewModel
{
    public IEnumerable<FeaturedPostDto> FeaturedPosts { get; set; } = new List<FeaturedPostDto>();
}