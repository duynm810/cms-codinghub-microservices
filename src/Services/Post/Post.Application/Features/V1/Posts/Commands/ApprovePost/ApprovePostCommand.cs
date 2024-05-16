using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.ApprovePost;

public class ApprovePostCommand(Guid id) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;
}