using Shared.Dtos.Comment;
using Shared.Responses;
using WebApps.UI.ApiServices.Interfaces;

namespace WebApps.UI.ApiServices;

public class CommentApiClient(IBaseApiClient baseApiClient) : ICommentApiClient
{
    public async Task<ApiResult<List<CommentDto>>> GetCommentsByPostId(Guid postId)
    {
        return await baseApiClient.GetListAsync<CommentDto>($"/comments/by-post/{postId}");
    }

    public async Task<ApiResult<CommentDto>> CreateComment(CreateCommentDto comment)
    {
        return await baseApiClient.PostAsync<CreateCommentDto, CommentDto>($"/comments", comment);
    }
}