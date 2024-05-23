using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.ApprovePost;

public class ApprovePostCommand(Guid id, Guid currentUserId) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = currentUserId;
}