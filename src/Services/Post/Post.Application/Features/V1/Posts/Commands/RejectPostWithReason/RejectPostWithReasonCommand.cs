using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(Guid id, string note) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public string? Note { get; set; } = note;
}