using MediatR;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQuery(Guid id) : IRequest<ApiResult<PostDto>>
{
    public Guid Id { get; set; } = id;
}