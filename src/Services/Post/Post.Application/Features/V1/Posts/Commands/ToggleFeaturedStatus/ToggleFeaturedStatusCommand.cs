using MediatR;
using Shared.Requests.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.ToggleFeaturedStatus;

public class ToggleFeaturedStatusCommand(Guid id, ToggleFeaturedStatusRequest toggleFeaturedStatus) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; } = id;

    public ToggleFeaturedStatusRequest ToggleFeaturedStatus { get; private set; } = toggleFeaturedStatus;
}