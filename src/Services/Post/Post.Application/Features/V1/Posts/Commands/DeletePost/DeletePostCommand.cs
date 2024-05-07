using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.DeletePost;

public class DeletePostCommand(Guid id) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; } = id;
}