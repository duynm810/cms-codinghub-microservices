using Shared.Dtos.Post;
using Shared.Responses;
using WebApps.UI.Services.Interfaces;

namespace WebApps.UI.Services;

public class PostApiClient(IBaseApiClient baseApiClient) : IPostApiClient
{
    public async Task<ApiResult<List<FeaturedPostDto>>> GetFeaturedPosts(int count)
    {
        return await baseApiClient.GetListAsync<FeaturedPostDto>($"/posts/featured?count={count}");
    }
}