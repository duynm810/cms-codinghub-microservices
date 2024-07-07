using MediatR;
using Shared.Dtos.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.TogglePinStatus;

public class TogglePinStatusCommand(Guid id, TogglePinStatusDto togglePinStatus) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; } = id;

    public TogglePinStatusDto TogglePinStatus { get; } = togglePinStatus;
}