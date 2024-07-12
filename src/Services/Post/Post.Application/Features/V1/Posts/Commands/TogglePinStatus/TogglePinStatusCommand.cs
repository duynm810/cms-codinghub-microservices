using MediatR;
using Shared.Dtos.Post.Commands;
using Shared.Requests.Post.Commands;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.TogglePinStatus;

public class TogglePinStatusCommand(Guid id, TogglePinStatusRequest togglePinStatus) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; private set; } = id;

    public TogglePinStatusRequest TogglePinStatus { get; } = togglePinStatus;
}