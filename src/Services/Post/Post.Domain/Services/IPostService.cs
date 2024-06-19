using Post.Domain.Entities;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Domain.Services;

public interface IPostService
{
    Task<List<PostDto>> EnrichPostsWithCategories(IEnumerable<PostBase> postList, CancellationToken cancellationToken = default);
    
    Task<PagedResponse<PostDto>> EnrichPagedPostsWithCategories(PagedResponse<PostBase> pagedPosts, CancellationToken cancellationToken = default);
}