using MediatR;
using Shared.Dtos.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.ToggleFeaturedStatus;

public class ToggleFeaturedStatusCommand(Guid id, ToggleFeaturedStatusDto toggleFeaturedStatus) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; } = id;

    public ToggleFeaturedStatusDto ToggleFeaturedStatus { get; private set; } = toggleFeaturedStatus;
}