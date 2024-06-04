using MediatR;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(Guid id, Guid userId, RejectPostWithReasonDto request)
    : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public Guid UserId { get; set; } = userId;

    public string? Reason { get; set; } = request.Note;
}