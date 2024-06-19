using MediatR;
using Post.Application.Commons.Models;
using Shared.Dtos.Post;
using Shared.Dtos.Post.Queries;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQuery(Guid id) : IRequest<ApiResult<PostDto>>
{
    public Guid Id { get; set; } = id;
}