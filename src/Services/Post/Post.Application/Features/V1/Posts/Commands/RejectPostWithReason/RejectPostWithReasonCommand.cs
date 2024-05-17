using MediatR;
using Shared.Responses;

namespace Post.Application.Features.V1.Posts.Commands.RejectPostWithReason;

public class RejectPostWithReasonCommand(Guid id, string reason) : IRequest<ApiResult<bool>>
{
    public Guid Id { get; set; } = id;

    public string? Reason { get; set; } = reason;
}