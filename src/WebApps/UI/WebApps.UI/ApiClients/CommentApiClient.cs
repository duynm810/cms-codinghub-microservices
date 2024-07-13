using Shared.Dtos.Comment;
using Shared.Requests.Comment;
using Shared.Responses;
using WebApps.UI.ApiClients.Interfaces;

namespace WebApps.UI.ApiClients;

public class CommentApiClient(IBaseApiClient baseApiClient) : ICommentApiClient
{
    public async Task<ApiResult<List<CommentDto>>> GetCommentsByPostId(Guid postId)
    {
        return await baseApiClient.GetListAsync<CommentDto>($"/comments/by-post/{postId}");
    }
    
    public async Task<ApiResult<List<LatestCommentDto>>> GetLatestComments(int count)
    {
        return await baseApiClient.GetListAsync<LatestCommentDto>($"/comments/latest?count={count}");
    }

    public async Task<ApiResult<CommentDto>> CreateComment(CreateCommentRequest request)
    {
        return await baseApiClient.PostAsync<CreateCommentRequest, CommentDto>($"/comments", request, true);
    }
    
    public async Task<ApiResult<CommentDto>> ReplyToComment(string parentId, CreateCommentRequest request)
    {
        return await baseApiClient.PostAsync<CreateCommentRequest, CommentDto>($"/comments/reply?parentId={parentId}", request, true);
    }
}