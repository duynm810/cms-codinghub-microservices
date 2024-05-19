using MediatR;
using Shared.Dtos.Post;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(Guid id, RejectPostWithReasonDto model) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public string? Reason { get; set; } = model.Note;
}