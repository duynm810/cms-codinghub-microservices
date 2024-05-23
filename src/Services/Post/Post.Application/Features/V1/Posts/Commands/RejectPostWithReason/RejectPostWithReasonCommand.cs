using MediatR;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(Guid id, Guid currentUserId, RejectPostWithReasonDto model) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = currentUserId;

    public string? Reason { get; set; } = model.Note;
}