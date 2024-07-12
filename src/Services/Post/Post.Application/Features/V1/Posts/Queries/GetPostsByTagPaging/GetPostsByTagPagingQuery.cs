using MediatR;
using Shared.Dtos.Post;
using Shared.Requests.Post;
using Shared.Requests.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostsByTagPaging;

public class GetPostsByTagPagingQuery(string tagSlug, GetPostsByTagRequest request) : IRequest<ApiResult<PostsByTagDto>>
{
    public string TagSlug { get; set; } = tagSlug;

    public GetPostsByTagRequest Request { get; set; } = request;
}