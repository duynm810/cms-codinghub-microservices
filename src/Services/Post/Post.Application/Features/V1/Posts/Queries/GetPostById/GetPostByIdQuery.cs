using MediatR;
using Post.Application.Commons.Models;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Queries.GetPostById;

public class GetPostByIdQuery(Guid id) : IRequest<ApiResult<PostModel>>
{
    public Guid Id { get; set; } = id;
}