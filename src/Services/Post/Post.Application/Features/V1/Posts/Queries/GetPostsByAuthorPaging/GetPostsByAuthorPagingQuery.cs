using MediatR;
using Shared.Dtos.Post.Queries;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByAuthorPaging;

public class GetPostsByAuthorPagingQuery(string username, GetPostsByAuthorRequest request) : IRequest<ApiResult<PostsByAuthorDto>>
{
    public string UserName { get; set; } = username;
    
    public GetPostsByAuthorRequest Request { get; set; } = request;
}