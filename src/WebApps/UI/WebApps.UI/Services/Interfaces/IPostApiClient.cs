using Shared.Dtos.Post;
using Shared.Responses;

namespace WebApps.UI.Services.Interfaces;

public interface IPostApiClient
{
    Task<ApiResult<List<FeaturedPostDto>>> GetFeaturedPosts(int count);
}